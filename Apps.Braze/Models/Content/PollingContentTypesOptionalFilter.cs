using Apps.Braze.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Braze.Models.Content
{
    public class PollingContentTypesOptionalFilter
    {
        [Display("Content types"), StaticDataSource(typeof(BrazeContentTypeDataSourceHandler))]
        public IEnumerable<string> ContentTypes { get; set; }
    }
}
