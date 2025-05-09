using Apps.Braze.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Braze.Models.EmailTemplates
{
    public class SearchEmailTemplatesRequest
    {
        [Display("Modified after")]
        public DateTime? ModifiedAfter { get; set; }

        [Display("Modified before")]
        public DateTime? ModifiedBefore { get; set; }

        [Display("Limit")]
        public int? Limit { get; set; }

        [Display("Offset")]
        public int? Offset { get; set; }
    }
}
