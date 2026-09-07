using Apps.Braze.Models.ContentBlocks;
using Apps.Braze.Polling.Memory;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Braze.Polling;

[PollingEventList("Content blocks")]
public class ContentBlockPollingList(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [PollingEvent("On content block tag added", Description = "Triggers when a content block tag is added")]
    public async Task<PollingEventResponse<DateMemory, ContentBlockPollingResponse>> OnContentBlockTagAdded(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] PollingContentBlockRequest input)
    {
        var contentBlocks = await GetContentBlocks(request.Memory?.LastInteractionDate);

        if (!string.IsNullOrWhiteSpace(input.ContentBlockId))
        {
            contentBlocks = contentBlocks
                .Where(block => string.Equals(
                    block.ContentBlockId,
                    input.ContentBlockId,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (input.Tags?.Any() == true)
        {
            var requiredTags = new HashSet<string>(input.Tags, StringComparer.OrdinalIgnoreCase);
            contentBlocks = contentBlocks
                .Where(block => block.Tags?.Any(tag => requiredTags.Contains(tag)) == true)
                .ToList();
        }

        if (contentBlocks.Count == 0)
        {
            return new PollingEventResponse<DateMemory, ContentBlockPollingResponse>
            {
                FlyBird = false,
                Memory = request.Memory ?? new DateMemory { LastInteractionDate = DateTime.UtcNow }
            };
        }

        if (request.Memory is null)
        {
            return new PollingEventResponse<DateMemory, ContentBlockPollingResponse>
            {
                FlyBird = false,
                Memory = new DateMemory
                {
                    LastInteractionDate = contentBlocks.Max(block => block.LastEdited)
                }
            };
        }

        request.Memory.LastInteractionDate = contentBlocks.Max(block => block.LastEdited);
        return new PollingEventResponse<DateMemory, ContentBlockPollingResponse>
        {
            FlyBird = true,
            Memory = request.Memory,
            Result = new ContentBlockPollingResponse
            {
                ContentBlocks = contentBlocks.OrderBy(block => block.LastEdited).ToList()
            }
        };
    }

    [PollingEvent("On content block updated", Description = "Triggers when one or more content blocks are updated")]
    public async Task<PollingEventResponse<DateMemory, ContentBlockPollingResponse>> OnContentBlockUpdated(
        PollingEventRequest<DateMemory> request)
    {
        if (request.Memory is null)
        {
            return new PollingEventResponse<DateMemory, ContentBlockPollingResponse>
            {
                FlyBird = false,
                Memory = new DateMemory { LastInteractionDate = DateTime.UtcNow }
            };
        }

        var contentBlocks = await GetContentBlocks(request.Memory.LastInteractionDate);
        if (contentBlocks.Count == 0)
        {
            return new PollingEventResponse<DateMemory, ContentBlockPollingResponse>
            {
                FlyBird = false,
                Memory = request.Memory
            };
        }

        request.Memory.LastInteractionDate = contentBlocks.Max(block => block.LastEdited);
        return new PollingEventResponse<DateMemory, ContentBlockPollingResponse>
        {
            FlyBird = true,
            Memory = request.Memory,
            Result = new ContentBlockPollingResponse
            {
                ContentBlocks = contentBlocks.OrderBy(block => block.LastEdited).ToList()
            }
        };
    }

    private async Task<List<ContentBlockListItem>> GetContentBlocks(DateTime? threshold)
    {
        const int pageSize = 1000;
        var offset = 0;
        var result = new List<ContentBlockListItem>();

        while (true)
        {
            var request = new RestRequest("/content_blocks/list", Method.Get)
                .AddQueryParameter("limit", pageSize.ToString());
            if (threshold.HasValue)
                request.AddQueryParameter("modified_after", threshold.Value.ToString("o"));
            if (offset > 0)
                request.AddQueryParameter("offset", offset.ToString());

            var response = await Client.ExecuteWithErrorHandling<ContentBlockListResponse>(request);
            var page = (response.ContentBlocks ?? []).ToList();
            result.AddRange(threshold.HasValue
                ? page.Where(block => block.LastEdited > threshold.Value)
                : page);

            if (page.Count < pageSize)
                break;

            offset += pageSize;
        }

        return result;
    }
}
