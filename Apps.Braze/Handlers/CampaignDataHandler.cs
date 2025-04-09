using Apps.Braze.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Braze.Handlers;
public class CampaignDataHandler(InvocationContext invocationContext) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new RestRequest("/campaigns/list");
        var response = await Client.ExecuteWithErrorHandling<CampaignListDto>(request);

        return response.Campaigns.Select(x => new DataSourceItem(x.Id, x.Name));
    }
}
