using Apps.Braze.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Braze.Models.Campaigns;
public class CampaignRequest
{
    [Display("Campaign ID")]
    [DataSource(typeof(CampaignDataHandler))]
    public string CampaignId { get; set; }
}
