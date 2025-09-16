using Apps.Braze.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Braze.Models.Content
{
    public class ContentTypeFilter
    {
        [Display("Content type"), StaticDataSource(typeof(BrazeContentTypeDataSourceHandler))]
        public virtual string ContentType { get; set; } = default!;
    }
}
