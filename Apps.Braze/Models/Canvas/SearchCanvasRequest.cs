using Blackbird.Applications.Sdk.Common;

namespace Apps.Braze.Models.Canvas
{
    public class SearchCanvasRequest
    {
        [Display("Edited after")]
        public DateTime? LastEdited { get; set; }
    }
}
