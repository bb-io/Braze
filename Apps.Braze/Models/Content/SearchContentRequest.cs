using Apps.Braze.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Braze.Models.Content
{
    public class SearchContentRequest
    {
        [Display("Content types")]
        [StaticDataSource(typeof(ContentTypeDataSourceHandler))]
        public string ContentType { get; set; }

        [Display("Edited after")]
        public DateTime? EditedAfter { get; set; }

        [Display("Edited before")]
        public DateTime? EditedBefore { get; set; }

        [Display("Limit (Email templates only)")]
        public int? Limit { get; set; }

        [Display("Offset (Email templates only)")]
        public int? Offset { get; set; }
    }
}
