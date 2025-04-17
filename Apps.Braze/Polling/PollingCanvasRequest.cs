using Apps.Braze.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Braze.Polling
{
    public class PollingCanvasRequest
    {
        [Display("Canvas ID")]
        [DataSource(typeof(CanvasDataHandler))]
        public string? CanvasId { get; set; }
    }
}
