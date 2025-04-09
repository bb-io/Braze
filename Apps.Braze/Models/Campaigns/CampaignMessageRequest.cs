using Apps.Braze.Handlers;
using Apps.Braze.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Models.Campaigns;
public class CampaignMessageRequest
{
    [Display("Campaign ID")]
    [DataSource(typeof(CampaignDataHandler))]
    public string CampaignId { get; set; }

    [Display("Message variation ID")]
    public string MessageVariationId { get; set; }

    [Display("Locale")]
    [StaticDataSource(typeof(LocaleDataHandler))]
    public string Locale { get; set; }
}
