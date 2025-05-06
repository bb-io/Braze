using Apps.Braze.Models.Campaigns;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Apps.Braze.Actions;
using Apps.Braze.Dtos;
using RestSharp;

namespace Apps.Braze.Handlers
{
    public class MessageVariationDataHandler : Invocable, IAsyncDataSourceHandler
    {
        readonly string _campaignId;
        public MessageVariationDataHandler(InvocationContext invocationContext,
        [ActionParameter] CampaignRequest request) : base(invocationContext)
        {
            _campaignId = request.CampaignId;
        }
        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,CancellationToken cancellationToken)
        {
            var request = new RestRequest("/campaigns/details");
            request.AddQueryParameter("campaign_id", _campaignId);
            var response = await Client.ExecuteWithErrorHandling<CampaignDto>(request);

            return response.MessageVariations
                    .Where(x => context.SearchString == null || x.Name.Contains(context.SearchString, StringComparison.InvariantCultureIgnoreCase))
                    .ToDictionary(x => x.Id, x => x.Name );
        }

    }
}

