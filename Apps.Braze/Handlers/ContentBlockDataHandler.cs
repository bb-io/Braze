using Apps.Braze.Models.ContentBlocks;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Braze.Handlers;

public class ContentBlockDataHandler(InvocationContext invocationContext) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var contentBlocks = await GetAllContentBlocksAsync();

        return contentBlocks
            .Where(x => string.IsNullOrEmpty(context.SearchString)
                        || x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.Name)
            .Select(x => new DataSourceItem(x.ContentBlockId, x.Name));
    }

    private async Task<List<ContentBlockListItem>> GetAllContentBlocksAsync()
    {
        const int pageSize = 1000;
        var offset = 0;
        var result = new List<ContentBlockListItem>();

        while (true)
        {
            var request = new RestRequest("/content_blocks/list")
                .AddQueryParameter("limit", pageSize.ToString());
            if (offset > 0)
                request.AddQueryParameter("offset", offset.ToString());

            var response = await Client.ExecuteWithErrorHandling<ContentBlockListResponse>(request);
            var page = (response.ContentBlocks ?? []).ToList();
            result.AddRange(page);

            if (page.Count < pageSize)
                return result;

            offset += pageSize;
        }
    }
}
