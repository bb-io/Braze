using Apps.Braze.Models.Canvas;

namespace Apps.Braze.Polling
{
    public class PollingCanvasResponse
    {
        public IEnumerable<CanvasDto> Canvases { get; }

        public PollingCanvasResponse(IEnumerable<CanvasDto> canvases)
        {
            Canvases = canvases;
        }
    }
}
