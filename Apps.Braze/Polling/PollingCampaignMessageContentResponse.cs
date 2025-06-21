using Apps.Braze.Dtos;

namespace Apps.Braze.Polling
{
    public class PollingCampaignMessageContentResponse
    {
        public IEnumerable<ListCampaignMessageDto> Messages { get; }

        public PollingCampaignMessageContentResponse(IEnumerable<ListCampaignMessageDto> messages)
        {
            Messages = messages;
        }
    }
}
