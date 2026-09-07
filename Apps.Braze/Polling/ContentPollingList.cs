using Apps.Braze.Dtos;
using Apps.Braze.Models.Canvas;
using Apps.Braze.Models.Content;
using Apps.Braze.Models.ContentBlocks;
using Apps.Braze.Polling.Memory;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Blackbird.Applications.SDK.Blueprints;
using RestSharp;

namespace Apps.Braze.Polling
{
    [PollingEventList("Content")]
    public class ContentPollingList(InvocationContext invocationContext) : Invocable(invocationContext)
    {
        [BlueprintEventDefinition(BlueprintEvent.ContentCreatedOrUpdatedMultiple)]
        [PollingEvent("On content updated", Description = "Triggers with a list of campaigns, canvases, and content blocks updated")]
        public Task<PollingEventResponse<DateMemory, ContentUpdatedMultipleResponse>> OnContentCreatedOrUpdatedMultiple(PollingEventRequest<DateMemory> request,
            [PollingEventParameter] PollingContentTypesOptionalFilter filter)
            => HandleContentUpdatedMultipleAsync(request, filter);

        private async Task<PollingEventResponse<DateMemory, ContentUpdatedMultipleResponse>> HandleContentUpdatedMultipleAsync(PollingEventRequest<DateMemory> request, PollingContentTypesOptionalFilter filter)
        {
            var isFirstRun = request.Memory is null;
            var memory = InitializeMemory(request.Memory);
            if (isFirstRun)
            {
                return new PollingEventResponse<DateMemory, ContentUpdatedMultipleResponse>
                {
                    FlyBird = false,
                    Memory = memory
                };
            }

            var result = await FetchUpdatedAsync(memory, filter);

            if (!result.HasUpdates)
            {
                return new PollingEventResponse<DateMemory, ContentUpdatedMultipleResponse>
                {
                    FlyBird = false,
                    Memory = result.Memory
                };
            }

            result.Memory.LastInteractionDate = result.Updated.Max(x => x.LastEdited);

            var items = result.Updated
                .OrderBy(x => x.LastEdited)
                .Select(x => x.Item)
                .ToList();

            return new PollingEventResponse<DateMemory, ContentUpdatedMultipleResponse>
            {
                FlyBird = true,
                Memory = result.Memory,
                Result = new ContentUpdatedMultipleResponse { Items = items }
            };
        }

        private async Task<FetchUpdatedResult> FetchUpdatedAsync(DateMemory memory, PollingContentTypesOptionalFilter filter)
        {
            var updated = new List<UpdatedRow>();
            var sinceIso = memory.LastInteractionDate.ToString("o");

            if (IsRequested(filter, "campaign"))
                updated.AddRange(await GetUpdatedCampaignRowsAsync(sinceIso, memory.LastInteractionDate));

            if (IsRequested(filter, "canvas"))
                updated.AddRange(await GetUpdatedCanvasRowsAsync(sinceIso, memory.LastInteractionDate));

            if (IsRequested(filter, "content_block"))
                updated.AddRange(await GetUpdatedContentBlockRowsAsync(sinceIso, memory.LastInteractionDate));

            return new FetchUpdatedResult(memory, updated);
        }

        private bool IsRequested(PollingContentTypesOptionalFilter filter, string type) =>
            filter.ContentTypes == null || filter.ContentTypes.Contains(type, StringComparer.OrdinalIgnoreCase);

        private DateMemory InitializeMemory(DateMemory? previous) =>
            new() { LastInteractionDate = previous?.LastInteractionDate ?? DateTime.UtcNow };

        private async Task<List<UpdatedRow>> GetUpdatedCampaignRowsAsync(string sinceIso, DateTime threshold)
        {
            var requeqst = new RestRequest("/campaigns/list", Method.Get)
                .AddQueryParameter("last_edit.time[gt]", sinceIso);

            var response = await Client.ExecuteWithErrorHandling<CampaignListDto>(requeqst);
            var campaigns = response?.Campaigns?.Where(c => c.LastEdited > threshold).ToList() ?? new();

            return campaigns.Select(c => new UpdatedRow(new ContentUpdatedItem
            {
                ContentId = c.Id,
                ContentType = "campaign",
                Name = c.Name,
                LastEdited = c.LastEdited,
                Tags = c.Tags,
                IsApiCampaign = c.IsApicampaign
            }, c.LastEdited)).ToList();
        }

        private async Task<List<UpdatedRow>> GetUpdatedCanvasRowsAsync(string sinceIso, DateTime threshold)
        {
            var request = new RestRequest("/canvas/list", Method.Get)
                .AddQueryParameter("last_edit.time[gt]", sinceIso);

            var response = await Client.ExecuteWithErrorHandling<CanvasListDto>(request);
            var canvases = response?.Canvases?.Where(c => c.LastEdited > threshold).ToList() ?? new();

            return canvases.Select(c => new UpdatedRow(new ContentUpdatedItem
            {
                ContentId = c.Id,
                ContentType = "canvas",
                Name = c.Name,
                LastEdited = c.LastEdited,
                Tags = c.Tags
            }, c.LastEdited)).ToList();
        }

        private async Task<List<UpdatedRow>> GetUpdatedContentBlockRowsAsync(string sinceIso, DateTime threshold)
        {
            const int pageSize = 1000;
            var offset = 0;
            var contentBlocks = new List<ContentBlockListItem>();

            while (true)
            {
                var request = new RestRequest("/content_blocks/list", Method.Get)
                    .AddQueryParameter("modified_after", sinceIso)
                    .AddQueryParameter("limit", pageSize.ToString());
                if (offset > 0)
                    request.AddQueryParameter("offset", offset.ToString());

                var response = await Client.ExecuteWithErrorHandling<ContentBlockListResponse>(request);
                var page = (response.ContentBlocks ?? []).ToList();
                contentBlocks.AddRange(page);

                if (page.Count < pageSize)
                    break;

                offset += pageSize;
            }

            return contentBlocks
                .Where(block => block.LastEdited > threshold)
                .Select(block => new UpdatedRow(new ContentUpdatedItem
                {
                    ContentId = block.ContentBlockId,
                    ContentType = "content_block",
                    Name = block.Name,
                    LastEdited = block.LastEdited,
                    Tags = block.Tags
                }, block.LastEdited))
                .ToList();
        }
    }
}
