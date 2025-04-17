namespace Apps.Braze.Polling
{
    public class PollingCanvasTagResponse
    {
        public IEnumerable<string> NewTags { get; }

        public PollingCanvasTagResponse(IEnumerable<string> newTags)
        {
            NewTags = newTags;
        }
    }
}
