using Apps.Braze.Dtos;
using Apps.Braze.Models.Canvas;
using Apps.Braze.Polling.Memory;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Braze.Polling
{
    [PollingEventList]
    public class PollingList : Invocable
    {
        public PollingList(InvocationContext invocationContext) : base(invocationContext)
        {
        }


        [PollingEvent("On canvas updated", Description = "Triggers when a canvas is updated")]
        public Task<PollingEventResponse<DateMemory, PollingCanvasResponse>> OnCanvasUpdated(
            PollingEventRequest<DateMemory> request)
            => HandleCanvasUpdatedPolling(request);

        [PollingEvent("On campaign updated" , Description = "Triggers when a campaign is updated")]
        public Task<PollingEventResponse<DateMemory, PollingCampaignResponse>> OnCampaignUpdated(PollingEventRequest<DateMemory> request)
        {
            return HandleCampaignUpdatedPolling(request);
        }

        [PollingEvent("On campaign message tag added", Description = "Triggers when a campaign message tag is added")]
        public Task<PollingEventResponse<TagMemory, CampaignDto>> OnCampaignTagAdded(
            PollingEventRequest<TagMemory> request,
            [PollingEventParameter] PollingCampaignRequest input)
            => HandleCampaignTagPolling(request, input);

        [PollingEvent("On canvas message tag added", Description = "Triggers when a canvas message tag is added")]
        public Task<PollingEventResponse<TagMemory, CanvasDto>> OnCanvasTagAdded(
            PollingEventRequest<TagMemory> request,
            [PollingEventParameter] PollingCanvasRequest input)
            => HandleCanvasTagPolling(request, input);



        private async Task<PollingEventResponse<TagMemory, CanvasDto>> HandleCanvasTagPolling(
           PollingEventRequest<TagMemory> request,
           [PollingEventParameter]PollingCanvasRequest input)
        {
            var rest = new RestRequest("/canvas/details", Method.Get);
            rest.AddQueryParameter("canvas_id", input.CanvasId);
            var canvas = await Client.ExecuteWithErrorHandling<CanvasDto>(rest);

            var currentTags = canvas.Tags ?? new List<string>();

         

            if (request.Memory == null)
            {
                return new PollingEventResponse<TagMemory, CanvasDto>
                {
                    FlyBird = false,
                    Memory = new TagMemory { KnownTags = currentTags }
                };
            }

            var newTags = currentTags.Except(request.Memory.KnownTags).ToArray();
            if (newTags.Any())
            {
                request.Memory.KnownTags = currentTags;

                return new PollingEventResponse<TagMemory, CanvasDto>
                {
                    FlyBird = true,
                    Memory = request.Memory,
                    Result = canvas
                };
            }

            return new PollingEventResponse<TagMemory, CanvasDto>
            {
                FlyBird = false,
                Memory = request.Memory
            };
        }


        private async Task<PollingEventResponse<TagMemory, CampaignDto>> HandleCampaignTagPolling(
            PollingEventRequest<TagMemory> request,
            PollingCampaignRequest input)
        {
            var rest = new RestRequest("/campaigns/details", Method.Get);
            rest.AddQueryParameter("campaign_id", input.CampaignId);
            var campaign = await Client.ExecuteWithErrorHandling<CampaignDto>(rest);

            var currentTags = campaign.Tags ?? new List<string>();

            if (request.Memory == null)
            {
                return new PollingEventResponse<TagMemory, CampaignDto>
                {
                    FlyBird = false,
                    Memory = new TagMemory { KnownTags = currentTags.ToList() },
                    Result = campaign
                };
            }

            var memoryExcludingFilter = (input.Tags ?? Enumerable.Empty<string>())
                .Aggregate(
                    request.Memory.KnownTags,
                    (mem, tag) => mem
                        .Where(t => !t.Equals(tag, StringComparison.OrdinalIgnoreCase))
                        .ToList()
                );

            var newTags = currentTags
                .Except(memoryExcludingFilter, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (input.Tags?.Any() == true)
            {
                var filterSet = new HashSet<string>(input.Tags, StringComparer.OrdinalIgnoreCase);
                newTags = newTags
                    .Where(t => filterSet.Contains(t))
                    .ToList();
            }

            request.Memory.KnownTags = currentTags.ToList();

            return new PollingEventResponse<TagMemory, CampaignDto>
            {
                FlyBird = newTags.Any(),
                Memory = request.Memory,
                Result = campaign
            };
        }




        private async Task<PollingEventResponse<DateMemory, PollingCanvasResponse>> HandleCanvasUpdatedPolling(
           PollingEventRequest<DateMemory> request)
        {
            var restRequest = new RestRequest("/canvas/list", Method.Get);

            if (request.Memory != null)
            {
                restRequest.AddQueryParameter(
                    "last_edit.time[gt]",
                    request.Memory.LastInteractionDate.ToString("o")
                );
            }

            var response = await Client.ExecuteWithErrorHandling<CanvasListDto>(restRequest);
            var allCanvases = response.Canvases.ToArray();

            if (request.Memory == null)
            {
                var maxDate = allCanvases.Any()
                    ? allCanvases.Max(c => c.LastEdited)
                    : DateTime.UtcNow;

                return new PollingEventResponse<DateMemory, PollingCanvasResponse>
                {
                    FlyBird = false,
                    Memory = new DateMemory { LastInteractionDate = maxDate }
                };
            }

            var newCanvases = allCanvases
                .Where(c => c.LastEdited > request.Memory.LastInteractionDate)
                .ToArray();

            if (newCanvases.Any())
            {
                var newMax = newCanvases.Max(c => c.LastEdited);
                request.Memory.LastInteractionDate = newMax;

                return new PollingEventResponse<DateMemory, PollingCanvasResponse>
                {
                    FlyBird = true,
                    Memory = request.Memory,
                    Result = new PollingCanvasResponse(newCanvases)
                };
            }

            return new PollingEventResponse<DateMemory, PollingCanvasResponse>
            {
                FlyBird = false,
                Memory = request.Memory
            };
        }

        private async Task<PollingEventResponse<DateMemory, PollingCampaignResponse>> HandleCampaignUpdatedPolling(
            PollingEventRequest<DateMemory> request)
        {
            var restRequest = new RestRequest("/campaigns/list", Method.Get);
            if (request.Memory != null)
                restRequest.AddQueryParameter("last_edit.time[gt]", request.Memory.LastInteractionDate.ToString("o"));

            var response = await Client.ExecuteWithErrorHandling<CampaignListDto>(restRequest);
            var all = response.Campaigns.ToArray();

            if (request.Memory == null)
            {
                var max = all.Any() ? all.Max(c => c.LastEdited) : DateTime.UtcNow;
                return new PollingEventResponse<DateMemory, PollingCampaignResponse>
                {
                    FlyBird = false,
                    Memory = new DateMemory { LastInteractionDate = max }
                };
            }

            var news = all.Where(c => c.LastEdited > request.Memory.LastInteractionDate).ToArray();
            if (news.Any())
            {
                var max = news.Max(c => c.LastEdited);
                request.Memory.LastInteractionDate = max;
                return new PollingEventResponse<DateMemory, PollingCampaignResponse>
                {
                    FlyBird = true,
                    Memory = request.Memory,
                    Result = new PollingCampaignResponse(news)
                };
            }

            return new PollingEventResponse<DateMemory, PollingCampaignResponse>
            {
                FlyBird = false,
                Memory = request.Memory
            };
        }
    }

}
