using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Braze.Handlers.Static
{
    public class ContentTypeDataSourceHandler : IStaticDataSourceItemHandler
    {
        IEnumerable<DataSourceItem> IStaticDataSourceItemHandler.GetData()
        {
            return new List<DataSourceItem>
            {
                new() { Value = "campaign", DisplayName = "Campaign" },
                new() { Value = "canvas", DisplayName = "Canvas" },
                new() { Value = "email_template", DisplayName = "Email Template" }
            };
        }
    }
}
