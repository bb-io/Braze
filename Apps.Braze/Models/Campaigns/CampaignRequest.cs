using Apps.Braze.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Models.Campaigns;
public class CampaignRequest
{
    [Display("Campaign ID")]
    [DataSource(typeof(CampaignDataHandler))]
    public string CampaignId { get; set; }
}
