using Apps.Braze.Handlers.Static;
using Apps.Braze.Handlers;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Braze.Models.Campaigns;
public class UploadCampaignMessageRequest
{
    [Display("Campaign ID", Description = "Will be taken from the file metadata by default")]
    [DataSource(typeof(CampaignDataHandler))]
    public string? CampaignId { get; set; }

    [Display("Message variation ID", Description = "Will be taken from the file metadata by default")]
    public string? MessageVariationId { get; set; }

    [Display("Locale")]
    [StaticDataSource(typeof(LocaleDataHandler))]
    public string Locale { get; set; }

    [Display("Content file", Description = "Either a (translated) JSON or HTML file that originated from the Download action")]
    public FileReference File { get; set; }
}
