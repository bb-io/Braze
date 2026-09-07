using Apps.Braze.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Braze.Polling;

public class PollingContentBlockRequest
{
    [Display("Content block ID")]
    [DataSource(typeof(ContentBlockDataHandler))]
    public string? ContentBlockId { get; set; }

    [Display("Filter by tags")]
    public IEnumerable<string>? Tags { get; set; }
}
