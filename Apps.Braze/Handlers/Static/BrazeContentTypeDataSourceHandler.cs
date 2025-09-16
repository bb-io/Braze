using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Braze.Handlers.Static
{
    public class BrazeContentTypeDataSourceHandler : IStaticDataSourceHandler
    {
        public Dictionary<string, string> GetData() => new()
        {
            { "campaign", "Campaigns" },
            { "canvas", "Canvases" },
            { "email_template", "Email templates" }
        };
    }
}
