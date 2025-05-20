using Apps.Braze.Dtos;

namespace Apps.Braze.Polling
{
    public class PollingEmailTemplateResponse
    {
        public IEnumerable<ListTemplate> Templates { get; }

        public PollingEmailTemplateResponse(IEnumerable<ListTemplate> templates)
        {
            Templates = templates;
        }
    }
}
