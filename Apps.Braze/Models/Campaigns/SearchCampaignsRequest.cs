using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Models.Campaigns;
public class SearchCampaignsRequest
{
    [Display("Edited after")]
    public DateTime? LastEdited { get; set; }
}
