using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Braze.Models.Content
{
    public class DownloadContentResponse : IDownloadContentOutput
    {
        [Display("Content (HTML)")] public FileReference Content { get; set; }
        [Display("Content (JSON)")] public FileReference JsonFile { get; set; }
    }
}
