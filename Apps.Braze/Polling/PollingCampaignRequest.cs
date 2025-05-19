using Apps.Braze.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Braze.Polling
{
    public class PollingCampaignRequest
    {
        [Display("Campaign ID")]
        [DataSource(typeof(CampaignDataHandler))]
        public string? CampaignId { get; set; }

        [Display("Filter by tags")]
        public IEnumerable<string> Tags { get; set; }
    }
}
