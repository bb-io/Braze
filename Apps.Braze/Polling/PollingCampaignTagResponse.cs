namespace Apps.Braze.Polling
{
    public class PollingCampaignTagResponse
    {
        public IEnumerable<string> NewTags { get; }

        public PollingCampaignTagResponse(IEnumerable<string> newTags)
        {
            NewTags = newTags;
        }
    }
}
