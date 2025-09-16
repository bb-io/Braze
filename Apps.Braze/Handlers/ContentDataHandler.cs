using Apps.Braze.Dtos;
using Apps.Braze.Models.Canvas;
using Apps.Braze.Models.Content;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Braze.Handlers
{
    public class ContentDataHandler(InvocationContext invocationContext, [ActionParameter] ContentTypeFilter filter) : Invocable(invocationContext), IAsyncDataSourceItemHandler
    {
        public async Task<IEnumerable<DataSourceItem>> GetDataAsync(
       DataSourceContext context, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(filter.ContentType))
                throw new PluginMisconfigurationException("Please select 'Content type' first.");

            var search = context.SearchString ?? string.Empty;

            switch (filter.ContentType.Trim().ToLowerInvariant())
            {
                case "campaign":
                    {
                        var req = new RestRequest("/campaigns/list");
                        var res = await Client.ExecuteWithErrorHandling<CampaignListDto>(req);
                        return (res?.Campaigns ?? Enumerable.Empty<ListCampaign>())
                            .Where(c => string.IsNullOrEmpty(search) || c.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                            .Select(c => new DataSourceItem { Value = c.Id, DisplayName = c.Name });
                    }
                case "canvas":
                    {
                        var req = new RestRequest("/canvas/list");
                        var res = await Client.ExecuteWithErrorHandling<CanvasListDto>(req);
                        return (res?.Canvases ?? Enumerable.Empty<CanvasDto>())
                            .Where(c => string.IsNullOrEmpty(search) || c.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                            .Select(c => new DataSourceItem { Value = c.Id, DisplayName = c.Name });
                    }
                case "email_template":
                    {
                        var req = new RestRequest("/templates/email/list");
                        req.AddQueryParameter("limit", "20");
                        var res = await Client.ExecuteWithErrorHandling<EmailTemplateListDto>(req);

                        var list = res?.Templates ?? Enumerable.Empty<ListTemplate>();
                        return list
                            .Where(t => string.IsNullOrEmpty(search)
                                     || t.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                            .Select(t => new DataSourceItem { Value = t.Id, DisplayName = t.Name });
                    }
                default:
                    throw new PluginMisconfigurationException("Unsupported Content type. Valid: campaign | canvas | email_template.");
            }
        }
    }
}
