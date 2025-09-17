using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Models.Content
{
    public class ContentUpdatedMultipleResponse
    {
        [Display("Items")]
        public IEnumerable<ContentUpdatedItem> Items { get; set; } = Enumerable.Empty<ContentUpdatedItem>();
    }

    public class ContentUpdatedItem : IDownloadContentInput
    {
        [Display("Content ID")]
        public string ContentId { get; set; } = default!;

        [Display("Content type")]
        public string ContentType { get; set; } = default!;

        [Display("Name")]
        public string? Name { get; set; }

        [Display("Last edited")]
        public DateTime? LastEdited { get; set; }

        [Display("Tags")]
        public IEnumerable<string>? Tags { get; set; }

        [Display("Is API campaign?")]
        public bool? IsApiCampaign { get; set; }
    }
}
