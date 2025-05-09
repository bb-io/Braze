using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Apps.Braze.Models.EmailTemplates
{
    public class CreateEmailTemplateRequest
    {
        [Display("Template name")]
        public string TemplateName { get; set; }

        [Display("Subject")]
        public string Subject { get; set; }

        [Display("Body string")]
        public string? StringBody { get; set; }

        [Display("Body HTML file")]
        public FileReference? HtmlBody { get; set; }

        [Display("Plain text body")]
        public string? PlaintextBody { get; set; }

        [Display("Preheader")]
        public string? Preheader { get; set; }
        public IEnumerable<string>? Tags { get; set; }
    }

    public class CreateEmailTemplateResponseDto
    {
        [JsonProperty("email_template_id")]
        public string EmailTemplateId { get; set; }
    }
    public class CreateEmailTemplateResponse
    {
        [Display("Email template ID")]
        public string EmailTemplateId { get; set; }
    }
}
