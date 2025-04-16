using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Models.Canvas
{
    public class CanvasDetailsDto
    {
        [Display("Created At")]
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [Display("Updated at")]
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Display("Name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Display("Description")]
        [JsonProperty("description")]
        public string Description { get; set; }

        [Display("Archived")]
        [JsonProperty("archived")]
        public bool Archived { get; set; }

        [Display("Draft")]
        [JsonProperty("draft")]
        public bool Draft { get; set; }

        [Display("Schedule type")]
        [JsonProperty("schedule_type")]
        public string ScheduleType { get; set; }

        [Display("First entry")]
        [JsonProperty("first_entry")]
        public DateTime? FirstEntry { get; set; }

        [Display("Last entry")]
        [JsonProperty("last_entry")]
        public DateTime? LastEntry { get; set; }

        [Display("Channels")]
        [JsonProperty("channels")]
        public List<string> Channels { get; set; }

        [Display("Variants")]
        [JsonProperty("variants")]
        public List<CanvasVariantDto> Variants { get; set; }

        [Display("Tags")]
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [Display("Teams")]
        [JsonProperty("teams")]
        public List<string> Teams { get; set; }

        [Display("Steps")]
        [JsonProperty("steps")]
        public List<CanvasStepDto> Steps { get; set; }

        [Display("Message")]
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class CanvasVariantDto
    {
        [Display("Variant name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Display("Variant ID")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [Display("First step IDs")]
        [JsonProperty("first_step_ids")]
        public List<string> FirstStepIds { get; set; }
    }

    public class CanvasStepDto
    {
        [Display("Step name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Display("Step type")]
        [JsonProperty("type")]
        public string Type { get; set; }

        [Display("Step ID")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [Display("Next step IDs")]
        [JsonProperty("next_step_ids")]
        public List<string> NextStepIds { get; set; }

        [Display("Next paths")]
        [JsonProperty("next_paths")]
        public List<NextPathDto> NextPaths { get; set; }

        [Display("Step channels")]
        [JsonProperty("channels")]
        public List<string> Channels { get; set; }

        [Display("Messages")]
        [JsonProperty("messages")]
        public Dictionary<string, CanvasMessageDto> Messages { get; set; }
    }

    public class NextPathDto
    {
        [Display("Next path name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Display("Next step ID")]
        [JsonProperty("next_step_id")]
        public string NextStepId { get; set; }
    }
    public class CanvasMessageDto
    {
        [Display("Channel")]
        [JsonProperty("channel")]
        public string Channel { get; set; }

    }
}
