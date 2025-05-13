using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Braze.Models.EmailTemplates
{
    public class EmailTemplateResponse
    {
        [Display("Email template ID")]
        public string EmailTemplateId { get; set; }

        [Display("Template name")]
        public string TemplateName { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Preheader { get; set; }

        [Display("Plaintext body")]
        public string PlaintextBody { get; set; }

        [Display("Should inline CSS")]
        public bool ShouldInlineCss { get; set; }
        public List<string> Tags { get; set; }

        [Display("Created at")]
        public DateTime CreatedAt { get; set; }

        [Display("Updated at")]
        public DateTime UpdatedAt { get; set; }
        public string Message { get; set; }

        [Display("Body")]
        public FileReference Body { get; set; }
    }
}
