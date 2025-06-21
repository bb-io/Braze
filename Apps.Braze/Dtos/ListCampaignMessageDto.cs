using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
namespace Apps.Braze.Dtos;

public class ListCampaignMessageDto
{

    [Display("Message variant")]
    public MessageListDto? Message { get; set; }

    [Display("Campaign")]
    public ListCampaign? Campaign { get; set; }
}

public class MessageListDto
{
    [Display("Message variation ID")]
    public string? Id { get; set; }

    [Display("Name")]
    [JsonProperty("name")]
    public string? Name { get; set; }

    [Display("Channel")]
    [JsonProperty("channel")]
    public string? Channel { get; set; }
}
