using Apps.Braze.Handlers;
using Apps.Braze.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Braze.Models.Campaigns;
public class CampaignMessageRequest
{
    [Display("Campaign ID")]
    [DataSource(typeof(CampaignDataHandler))]
    public string CampaignId { get; set; }

    [Display("Message variation ID")]
    [DataSource(typeof(MessageVariationDataHandler))]
    public string? MessageVariationId { get; set; }

    [Display("Locale")]
    [StaticDataSource(typeof(LocaleDataHandler))]
    public string Locale { get; set; }
}
