using Apps.Braze.Models.Canvas;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Braze.Handlers
{
    public class CanvasDataHandler(InvocationContext invocationContext) : Invocable(invocationContext), IAsyncDataSourceItemHandler
    {
        public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var request = new RestRequest("/canvas/list");
            var response = await Client.ExecuteWithErrorHandling<CanvasListDto>(request);

            return response.Canvases
                .Where(x => context.SearchString == null || x.Name.Contains(context.SearchString, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => new DataSourceItem(x.Id, x.Name));
        }
    }
}
