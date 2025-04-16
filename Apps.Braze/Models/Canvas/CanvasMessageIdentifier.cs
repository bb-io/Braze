using Apps.Braze.Models.General;
using Newtonsoft.Json;

namespace Apps.Braze.Models.Canvas
{
    public class CanvasMessageIdentifier : IIdentifier
    {
        [JsonProperty("workflow_id")]
        public string CanvasId { get; set; }

        [JsonProperty("message_variation_id")]
        public string MessageVariationId { get; set; }

        [JsonProperty("step_id")]
        public string StepId { get; set; }

        public string GetId()
        {
            return MessageVariationId;
        }
    }
}
