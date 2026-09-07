using Apps.Braze.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Braze.Models.ContentBlocks;

public class SearchContentBlocksRequest
{
    [Display("Modified after")]
    public DateTime? ModifiedAfter { get; set; }

    [Display("Modified before")]
    public DateTime? ModifiedBefore { get; set; }
}

public class ContentBlockRequest
{
    [Display("Content block ID")]
    [DataSource(typeof(ContentBlockDataHandler))]
    public string ContentBlockId { get; set; } = default!;

    [Display("Include inclusion data")]
    public bool? IncludeInclusionData { get; set; }
}

public class UploadContentBlockRequest
{
    [Display("Content block ID")]
    [DataSource(typeof(ContentBlockDataHandler))]
    public string ContentBlockId { get; set; } = default!;

    [Display("Content file")]
    public FileReference Content { get; set; } = default!;
}
