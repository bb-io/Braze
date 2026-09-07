using Blackbird.Applications.Sdk.Common;

namespace Apps.Braze.Models.ContentBlocks;

public class ContentBlockPollingResponse
{
    [Display("Content blocks")]
    public IEnumerable<ContentBlockListItem> ContentBlocks { get; set; } = [];
}
