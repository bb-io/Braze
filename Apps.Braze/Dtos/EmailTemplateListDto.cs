using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Braze.Dtos;
public class EmailTemplateListDto
{
    [JsonProperty("count")]
    public int Count { get; set; }

    public IEnumerable<ListTemplate> Templates { get; set; }
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