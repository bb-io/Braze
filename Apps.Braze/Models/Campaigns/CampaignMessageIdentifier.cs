using Apps.Braze.Models.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
