using Apps.Braze.Dtos;

namespace Apps.Braze.Polling
{
    public class PollingCampaignResponse
    {
        public IEnumerable<ListCampaign> Campaigns { get; }

        public PollingCampaignResponse(IEnumerable<ListCampaign> campaigns)
        {
            Campaigns = campaigns;
        }
    }
}
