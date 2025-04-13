using Apps.Braze.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Braze.Models.EmailTemplates;
public class EmailTemplateRequest
{
    [Display("Email template ID")]
    [DataSource(typeof(EmailTemplateDataHandler))]
    public string EmailTemplateId { get; set; }
}
