using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Braze.Dtos;
public class CampaignDto
{
    [Display("Created at")]
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [Display("Updated at")]
    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Display("Name")]
    [JsonProperty("name")]
    public string Name { get; set; }

    [Display("Description")]
    [JsonProperty("description")]
    public string Description { get; set; }

    [Display("Archived?")]
    [JsonProperty("archived")]
    public bool Archived { get; set; }

    [Display("Enabled?")]
    [JsonProperty("enabled")]
    public bool Enabled { get; set; }

    [Display("Draft?")]
    [JsonProperty("draft")]
    public bool Draft { get; set; }

    [Display("Schedule type")]
    [JsonProperty("schedule_type")]
    public string ScheduleType { get; set; }

    [Display("Channels")]
    [JsonProperty("channels")]
    public List<string> Channels { get; set; }

    [Display("Tags")]
    [JsonProperty("tags")]
    public List<string> Tags { get; set; }

    [DefinitionIgnore]
    [JsonProperty("messages")]
    public Dictionary<string, Message> OriginalMessages { get; set; }

    [Display("Message variations")]
    public IEnumerable<Message> MessageVariations => OriginalMessages.Where(x => x.Value.Type != "control").Select(x =>
    {
        var val = x.Value;
        val.Id = x.Key;
        return val;
    });
}

public class Message
{
    [Display("Message variation ID")]
    public string Id { get; set; }

    [Display("Channel")]
    [JsonProperty("channel")]
    public string Channel { get; set; }

    [Display("Name")]
    [JsonProperty("name")]
    public string Name { get; set; }

    [Display("Subject")]
    [JsonProperty("subject")]
    public string Subject { get; set; }

    [Display("Body")]
    [JsonProperty("body")]
    public string Body { get; set; }

    [Display("From")]
    [JsonProperty("from")]
    public string From { get; set; }

    [Display("Reply to")]
    [JsonProperty("reply_to")]
    public string ReplyTo { get; set; }

    [Display("Title")]
    [JsonProperty("title")]
    public string Title { get; set; }

    [Display("Amp body")]
    [JsonProperty("amp_body")]
    public string AmpBody { get; set; }

    [Display("Preheader")]
    [JsonProperty("preheader")]
    public string Preheader { get; set; }

    [Display("Custom plain text")]
    [JsonProperty("custom_plain_text")]
    public string CustomPlainText { get; set; }

    [Display("Should inline CSS?")]
    [JsonProperty("should_inline_css")]
    public bool ShouldInlineCss { get; set; }

    [Display("Should whitespace preheader?")]
    [JsonProperty("should_whitespace_preheader")]
    public bool ShouldWhitespacePreheader { get; set; }

    [DefinitionIgnore]
    [JsonProperty("type")]
    public string Type { get; set; }
}
