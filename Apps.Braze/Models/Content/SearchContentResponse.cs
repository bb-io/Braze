using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Braze.Models.Content
{
    public class SearchContentResponse
    {
        [Display("Items")]
        public IEnumerable<ContentItem> Items { get; set; } = Enumerable.Empty<ContentItem>();
    }

    public class ContentItem : IDownloadContentInput
    {
        [Display("Content ID")] 
        public string ContentId { get; set; } = string.Empty;

        [Display("Content type")] 
        public string ContentType { get; set; }

        [Display("Name")] 
        public string? Name { get; set; }

        [Display("Tags")] 
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();

        [Display("Last edited")] 
        public DateTime? LastEdited { get; set; }

        [Display("Created at")] 
        public DateTime? CreatedAt { get; set; }
    }
}
