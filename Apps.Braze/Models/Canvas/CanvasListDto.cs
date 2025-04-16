using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Braze.Models.Canvas
{
    public class CanvasListDto
    {
        [JsonProperty("canvases")]
        public IEnumerable<CanvasDto> Canvases { get; set; }
    }

    public class CanvasDto
    {
        [Display("Canvas API identifier")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [Display("Last edited")]
        [JsonProperty("last_edited")]
        public DateTime LastEdited { get; set; }

        [Display("Canvas name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Display("Tags")]
        [JsonProperty("tags")]
        public IEnumerable<string> Tags { get; set; }
    }
}
