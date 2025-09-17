using Apps.Braze.Handlers;
using Apps.Braze.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Braze.Models.Content
{
    public class DownloadContentRequest : ContentTypeFilter, IDownloadContentInput
    {
        [Display("Content ID"), DataSource(typeof(ContentDataHandler))]
        public string ContentId { get; set; }

        [Display("Message variation ID")]
        public string? MessageVariationId { get; set; }

        [Display("Step ID")] 
        public string? StepId { get; set; }

        [Display("Locale")]
        [StaticDataSource(typeof(LocaleDataHandler))]
        public string? Locale { get; set; }
    }
}
