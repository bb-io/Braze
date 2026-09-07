using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Braze.Models.ContentBlocks;

public class ContentBlockListResponse
{
    [JsonProperty("count")]
    public int Count { get; set; }

    [Display("Content blocks")]
    [JsonProperty("content_blocks")]
    public IEnumerable<ContentBlockListItem> ContentBlocks { get; set; } = [];
}

public class ContentBlockListItem
{
    [Display("Content block ID")]
    [JsonProperty("content_block_id")]
    public string ContentBlockId { get; set; } = default!;

    [Display("Name")]
    [JsonProperty("name")]
    public string Name { get; set; } = default!;

    [Display("Content type")]
    [JsonProperty("content_type")]
    public string? ContentType { get; set; }

    [Display("Liquid tag")]
    [JsonProperty("liquid_tag")]
    public string? LiquidTag { get; set; }

    [Display("Inclusion count")]
    [JsonProperty("inclusion_count")]
    public int InclusionCount { get; set; }

    [Display("Created at")]
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [Display("Last edited")]
    [JsonProperty("last_edited")]
    public DateTime LastEdited { get; set; }

    [Display("Tags")]
    [JsonProperty("tags")]
    public IEnumerable<string> Tags { get; set; } = [];
}

public class ContentBlockInfo
{
    [Display("Content block ID")]
    [JsonProperty("content_block_id")]
    public string ContentBlockId { get; set; } = default!;

    [Display("Name")]
    [JsonProperty("name")]
    public string Name { get; set; } = default!;

    [Display("Content")]
    [JsonProperty("content")]
    public string Content { get; set; } = default!;

    [Display("Description")]
    [JsonProperty("description")]
    public string? Description { get; set; }

    [Display("Content type")]
    [JsonProperty("content_type")]
    public string? ContentType { get; set; }

    [Display("Tags")]
    [JsonProperty("tags")]
    public IEnumerable<string> Tags { get; set; } = [];

    [Display("Created at")]
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [Display("Last edited")]
    [JsonProperty("last_edited")]
    public DateTime LastEdited { get; set; }

    [Display("Inclusion count")]
    [JsonProperty("inclusion_count")]
    public int InclusionCount { get; set; }

    [Display("Inclusion data (JSON)")]
    [JsonProperty("inclusion_data_json")]
    public string? InclusionData { get; private set; }

    [JsonProperty("inclusion_data")]
    private JToken? InclusionDataToken
    {
        set => InclusionData = value?.ToString(Formatting.None);
    }

    [JsonProperty("message")]
    public string? Message { get; set; }
}
