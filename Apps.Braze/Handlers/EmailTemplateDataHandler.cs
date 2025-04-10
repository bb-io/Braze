using Apps.Braze.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Braze.Handlers;
public class EmailTemplateDataHandler(InvocationContext invocationContext) : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new RestRequest("/templates/email/list");
        var response = await Client.ExecuteWithErrorHandling<EmailTemplateListDto>(request);

        return response.Templates
            .Where(x => context.SearchString == null || x.Name.Contains(context.SearchString, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => new DataSourceItem(x.Id, x.Name));
    }
}
