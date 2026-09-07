using Apps.Braze.Dtos;
using Apps.Braze.Models.Canvas;
using Apps.Braze.Models.Content;
using Apps.Braze.Models.ContentBlocks;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Braze.Handlers
{
    public class ContentDataHandler(InvocationContext invocationContext, [ActionParameter] ContentTypeFilter filter) : Invocable(invocationContext), IAsyncDataSourceItemHandler
    {
        public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(filter.ContentType))
                throw new PluginMisconfigurationException("Please select 'Content type' first.");

            var search = context.SearchString ?? string.Empty;
            var type = filter.ContentType.Trim().ToLowerInvariant();

            return type switch
            {
                "campaign" => await GetCampaignsAsync(search),
                "canvas" => await GetCanvasesAsync(search),
                "email_template" => await GetEmailTemplatesAsync(search),
                "content_block" => await GetContentBlocksAsync(search),
                _ => throw new PluginMisconfigurationException("Unsupported Content type. Valid: campaign | canvas | email_template | content_block.")
            };
        }
        
        private async Task<IEnumerable<DataSourceItem>> GetCampaignsAsync(string search)
        {
            var request = new RestRequest("/campaigns/list");
            var response = await Client.ExecuteWithErrorHandling<CampaignListDto>(request);

            var items = (response?.Campaigns ?? Enumerable.Empty<ListCampaign>())
                .Where(c => string.IsNullOrEmpty(search)
                         || (c.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false))
                .Select(c => new DataSourceItem { Value = c.Id, DisplayName = c.Name });

            return items;
        }

        private async Task<IEnumerable<DataSourceItem>> GetCanvasesAsync(string search)
        {
            var request = new RestRequest("/canvas/list");
            var response = await Client.ExecuteWithErrorHandling<CanvasListDto>(request);

            var items = (response?.Canvases ?? Enumerable.Empty<CanvasDto>())
                .Where(c => string.IsNullOrEmpty(search)
                         || (c.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false))
                .Select(c => new DataSourceItem { Value = c.Id, DisplayName = c.Name });

            return items;
        }

        private async Task<IEnumerable<DataSourceItem>> GetEmailTemplatesAsync(string search)
        {
            var request = new RestRequest("/templates/email/list");
            request.AddQueryParameter("limit", "20");

            var response = await Client.ExecuteWithErrorHandling<EmailTemplateListDto>(request);

            var items = (response?.Templates ?? Enumerable.Empty<ListTemplate>())
                .Where(t => string.IsNullOrEmpty(search)
                         || (t.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false))
                .Select(t => new DataSourceItem { Value = t.Id, DisplayName = t.Name });

            return items;
        }

        private async Task<IEnumerable<DataSourceItem>> GetContentBlocksAsync(string search)
        {
            const int pageSize = 1000;
            var offset = 0;
            var contentBlocks = new List<ContentBlockListItem>();

            while (true)
            {
                var request = new RestRequest("/content_blocks/list")
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
                .Where(block => string.IsNullOrEmpty(search)
                                || block.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                .OrderBy(block => block.Name)
                .Select(block => new DataSourceItem
                {
                    Value = block.ContentBlockId,
                    DisplayName = block.Name
                });
        }
    }
}
