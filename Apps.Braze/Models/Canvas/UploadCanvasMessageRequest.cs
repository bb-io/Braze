using Apps.Braze.Handlers;
using Apps.Braze.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Braze.Models.Canvas
{
    public class UploadCanvasMessageRequest
    {
        [Display("Canvas ID", Description = "Will be taken from the file metadata by default")]
        [DataSource(typeof(CanvasDataHandler))]
        public string? CanvasId { get; set; }

        [Display("Step ID")]
        [DataSource(typeof(CanvasDataHandler))]
        public string? StepId { get; set; }

        [Display("Message variation ID", Description = "Will be taken from the file metadata by default")]
        public string? MessageVariationId { get; set; }

        [Display("Locale")]
        [StaticDataSource(typeof(LocaleDataHandler))]
        public string Locale { get; set; }

        [Display("Content file", Description = "Either a (translated) JSON or HTML file that originated from the Download action")]
        public FileReference File { get; set; }
    }
}
