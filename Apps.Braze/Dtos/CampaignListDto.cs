using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Dtos;
public class CampaignListDto
{
    public IEnumerable<ListCampaign> Campaigns { get; set; }
}

public class ListCampaign
{
    [Display("Campaign ID")]
    public string Id { get; set; }
    public string Name { get; set; }

    [Display("Is API campaign?")]
    [JsonProperty("is_api_campaign")]
    public bool IsApicampaign { get; set; }

    [Display("Tags")]
    public IEnumerable<string> Tags { get; set; }

    [Display("Last edited")]
    [JsonProperty("last_edited")]
    public DateTime LastEdited { get; set; }
}