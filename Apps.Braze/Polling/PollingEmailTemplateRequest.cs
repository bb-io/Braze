using Apps.Braze.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Braze.Polling
{
    public class PollingEmailTemplateRequest
    {
        [Display("Email template ID")]
        [DataSource(typeof(EmailTemplateDataHandler))]
        public string? EmailTemplateId { get; set; }

        [Display("Filter by tags")]
        public IEnumerable<string> Tags { get; set; }
    }
}
