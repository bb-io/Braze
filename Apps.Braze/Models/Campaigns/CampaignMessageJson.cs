using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Models.Campaigns;
public class CampaignMessageJson
{
    public string CampaignId { get; set; }
    public string MessageVariantId { get; set; }
    public Dictionary<string, string> TranslationMap { get; set; }
}
