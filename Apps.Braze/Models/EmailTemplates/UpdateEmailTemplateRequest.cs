using Apps.Braze.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Braze.Models.EmailTemplates
{
    public class UpdateEmailTemplateRequest
    {
        [Display("Email template ID")]
        [DataSource(typeof(EmailTemplateDataHandler))]
        public string EmailTemplateId { get; set; }

        [Display("Template name")]
        public string? TemplateName { get; set; }

        [Display("Subject")]
        public string? Subject { get; set; }

        [Display("Body string")]
        public string? StringBody { get; set; }

        [Display("Body HTML file")]
        public FileReference? HtmlBody { get; set; }

        [Display("Plain text body")]
        public string? PlaintextBody { get; set; }

        [Display("Preheader")]
        public string? Preheader { get; set; }

        [Display("Tags")]
        public IEnumerable<string>? Tags { get; set; }
    }
}
