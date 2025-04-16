using Apps.Braze.Models.General;
using Newtonsoft.Json;

namespace Apps.Braze.Models.Campaigns;
public class CampaignMessageIdentifier : IIdentifier
{
    [JsonProperty("campaign_id")]
    public string CampaignId { get; set; }

    [JsonProperty("message_variation_id")]
    public string MessageVariationId { get; set; }

    public string GetId()
    {
        return MessageVariationId;
    }
}
