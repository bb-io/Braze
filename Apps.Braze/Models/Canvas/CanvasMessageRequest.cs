using Apps.Braze.Handlers;
using Apps.Braze.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Braze.Models.Canvas
{
    public class CanvasMessageRequest
    {
        [Display("Canvas ID")]
        [DataSource(typeof(CanvasDataHandler))]
        public string CanvasId { get; set; }

        [Display("Step ID")]
        public string StepId { get; set; }

        [Display("Message variation ID")]
        public string MessageVariationId { get; set; }

        [Display("Locale")]
        [StaticDataSource(typeof(LocaleDataHandler))]
        public string Locale { get; set; }
    }
}
