using Apps.Braze.Dtos;
using Apps.Braze.Models.Canvas;
using Apps.Braze.Models.Content;
using Apps.Braze.Polling.Memory;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Blackbird.Applications.SDK.Blueprints;
using RestSharp;

namespace Apps.Braze.Polling
{
    [PollingEventList("Content")]
    public class ContentPollingList : Invocable
    {
        public ContentPollingList(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        [BlueprintEventDefinition(BlueprintEvent.ContentCreatedOrUpdatedMultiple)]
        [PollingEvent("On content updated",
            Description = "Triggers with a list of campaigns and canvases updated ")]
        public Task<PollingEventResponse<DateMemory, ContentUpdatedMultipleResponse>> OnContentCreatedOrUpdatedMultiple(
             PollingEventRequest<DateMemory> request,
             [PollingEventParameter] PollingContentTypesOptionalFilter filter)
             => HandleContentUpdatedMultiple(request, filter);


        private sealed record UpdatedRow(ContentUpdatedItem Item, DateTime LastEdited);


        private async Task<(DateMemory memory, List<UpdatedRow> updated, bool hasUpdates)> FetchUpdatedAsync(PollingEventRequest<DateMemory> request, PollingContentTypesOptionalFilter filter)
        {
            if (request.Memory is null)
            {
                return (new DateMemory { LastInteractionDate = DateTime.UtcNow }, new List<UpdatedRow>(), false);
            }

            var wantCampaign = filter.ContentTypes == null || filter.ContentTypes.Contains("campaign", StringComparer.OrdinalIgnoreCase);
            var wantCanvas = filter.ContentTypes == null || filter.ContentTypes.Contains("canvas", StringComparer.OrdinalIgnoreCase);

            var updated = new List<UpdatedRow>();
            var sinceIso = request.Memory.LastInteractionDate.ToString("o");

            if (wantCampaign)
            {
                var campaignsReq = new RestRequest("/campaigns/list", Method.Get)
                    .AddQueryParameter("last_edit.time[gt]", sinceIso);

                var campaignsRes = await Client.ExecuteWithErrorHandling<CampaignListDto>(campaignsReq);
                var campaigns = campaignsRes?.Campaigns?.Where(c => c.LastEdited > request.Memory.LastInteractionDate).ToList() ?? new();

                updated.AddRange(
                   campaigns.Select(c =>
                   {
                       var item = new ContentUpdatedItem
                       {
                           ContentId = c.Id,
                           ContentType = "campaign",
                           Name = c.Name,
                           LastEdited = c.LastEdited,
                           Tags = c.Tags,
                           IsApiCampaign = c.IsApicampaign
                       };
                       return new UpdatedRow(item, c.LastEdited);
                   }));
            }
            if (wantCanvas)
            {
                var canvasesReq = new RestRequest("/canvas/list", Method.Get)
                    .AddQueryParameter("last_edit.time[gt]", sinceIso);

                var canvasesRes = await Client.ExecuteWithErrorHandling<CanvasListDto>(canvasesReq);
                var canvases = canvasesRes?.Canvases?.Where(c => c.LastEdited > request.Memory.LastInteractionDate).ToList() ?? new();

                updated.AddRange(
            canvases.Select(c =>
            {
                var item = new ContentUpdatedItem
                {
                    ContentId = c.Id,
                    ContentType = "canvas",
                    Name = c.Name,
                    LastEdited = c.LastEdited,
                    Tags = c.Tags,
                };
                return new UpdatedRow(item, c.LastEdited);
                }));
            }

            if (!updated.Any())
                return (request.Memory, updated, false);

            request.Memory.LastInteractionDate = updated.Max(x => x.LastEdited);
            return (request.Memory, updated, true);
        }

        private async Task<PollingEventResponse<DateMemory, ContentUpdatedMultipleResponse>> HandleContentUpdatedMultiple(
            PollingEventRequest<DateMemory> request, PollingContentTypesOptionalFilter filter)
        {
            var (memory, updated, hasUpdates) = await FetchUpdatedAsync(request, filter);

            if (!hasUpdates)
                return new PollingEventResponse<DateMemory, ContentUpdatedMultipleResponse>
                {
                    FlyBird = false,
                    Memory = memory
                };

            var items = updated
                .OrderBy(x => x.LastEdited)
                .Select(x => x.Item)
                .ToList();

            return new PollingEventResponse<DateMemory, ContentUpdatedMultipleResponse>
            {
                FlyBird = true,
                Memory = memory,
                Result = new ContentUpdatedMultipleResponse { Items = items }
            };
        }
    }
}
