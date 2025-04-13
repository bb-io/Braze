using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Dtos;
public class EmailTemplateDto
{
    [JsonProperty("email_template_id")]
    public string EmailTemplateId { get; set; }

    [JsonProperty("template_name")]
    public string TemplateName { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("subject")]
    public string Subject { get; set; }

    [JsonProperty("preheader")]
    public string Preheader { get; set; }

    [JsonProperty("body")]
    public string Body { get; set; }

    [JsonProperty("plaintext_body")]
    public string PlaintextBody { get; set; }

    [JsonProperty("should_inline_css")]
    public bool ShouldInlineCss { get; set; }

    [JsonProperty("tags")]
    public List<string> Tags { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}
