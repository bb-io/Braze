using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Braze.Models.Content
{
    public class DownloadContentResponse : IDownloadContentOutput
    {
        [Display("(HTML)")] public FileReference Content { get; set; }
        [Display("(JSON)")] public FileReference JsonFile { get; set; }
    }
}
