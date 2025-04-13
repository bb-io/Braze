using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Dtos;
public class EmailTemplateListDto
{
    public List<ListTemplate> Templates { get; set; }
}

public class ListTemplate
{ 
    [Display("Email template ID")]
    [JsonProperty("email_template_id")]
    public string Id { get; set; }

    [JsonProperty("template_name")]
    public string Name { get; set; }

    [Display("Tags")]
    public IEnumerable<string> Tags { get; set; }

    [Display("Created at")]
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [Display("Updated at")]
    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }
}