using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Models.Campaigns;
public class CampaignFileResponse
{
    [Display("Campaign message (HTML)")]
    public FileReference HtmlFile { get; set; }

    [Display("Campaign message (JSON)")]
    public FileReference JsonFile { get; set; }
}
