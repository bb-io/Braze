using Apps.Braze.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Braze.Polling
{
    public class PollingCampaignMessageContentRequest
    {
        [Display("Filter by tags")]
        public IEnumerable<string> Tags { get; set; } = new List<string>();

        [Display("Locale")]
        [StaticDataSource(typeof(LocaleDataHandler))]
        public string Locale { get; set; } = string.Empty;
    }
}
