using Apps.Braze.Models.Content;
using Apps.Braze.Polling;
using Apps.Braze.Polling.Memory;
using Blackbird.Applications.Sdk.Common.Polling;
using Newtonsoft.Json;
using Tests.Braze.Base;

namespace Tests.Braze
{
    [TestClass]
    public class PollingTests : TestBase
    {
        [TestMethod]
        public async Task On_update_canvas_working()
        {
            var polling = new PollingList(InvocationContext);
            var oldDate = new DateTime(2024, 4, 1, 0, 0, 0, DateTimeKind.Utc);
            var request = new PollingEventRequest<DateMemory>
            {
                Memory = new DateMemory
                {
                    LastInteractionDate = oldDate
                }
            };

            var response = polling.OnCanvasUpdated(request);
            var canvases = response.Result.Result.Canvases;
            foreach (var canvas in canvases)
            {
                Console.WriteLine($"{canvas.Id} - {canvas.Name}");
            }
            
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task On_update_campaign_working()
        {
            var polling = new PollingList(InvocationContext);
            var oldDate = new DateTime(2024, 4, 1, 0, 0, 0, DateTimeKind.Utc);
            var request = new PollingEventRequest<DateMemory>
            {
                Memory = new DateMemory
                {
                    LastInteractionDate = oldDate
                }
            };

            var response = polling.OnCampaignUpdated(request);
            var canvases = response.Result.Result.Campaigns;
            foreach (var canvas in canvases)
            {
                Console.WriteLine($"{canvas.Id} - {canvas.Name}");
            }

            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task On_tags_update_campaign_working()
        {
            var polling = new PollingList(InvocationContext);
            var request = new PollingEventRequest<DateMemory>
            {
                Memory = new DateMemory
                {
                    LastInteractionDate = new DateTime(2025, 6, 26, 13, 20, 11, DateTimeKind.Utc)
                }
            };
            var campaign = new PollingCampaignRequest
            {
                //CampaignId= "841280d5-3693-4ff1-b32b-039dd4407670"
                Tags = new List<string> { "translate-email", "email" }
            };

            var response = await polling.OnCampaignTagAdded(request, campaign);

            var json = JsonConvert.SerializeObject(response, Formatting.Indented );
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task On_campaign_message_content_updated_working()
        {
            var polling = new PollingList(InvocationContext);
            var request = new PollingEventRequest<CampaignMessageMemory>
            {
                Memory = new CampaignMessageMemory
                {
                    LastInteractionDate = new DateTime(2025, 5, 5, 10, 50, 0, DateTimeKind.Utc),
                    CampaignMessages = new Dictionary<string, DateTime>()
                    {
                        { "083EC4E6FA1F4321D1874EBFC8E8F99E7B31016D5F55ACBCE205A876FA7B6A9C3174DE8185C64551BD3E7EFBB70D3C1F813661EFF108F50AA182D45C36E1B410", new DateTime(2025, 6, 21, 10, 52, 22, DateTimeKind.Utc) },
                        { "record-older-than-a-year-should-be-deleted-on-polling", new DateTime(2023, 6, 21, 10, 52, 22, DateTimeKind.Utc) }
                    }
                }
            };
            var campaign = new PollingCampaignMessageContentRequest
            {
                Tags = new List<string> { "email", "translate-email", "email_translate" },
                Locale = "en",
            };

            var response = await polling.OnCampaignMessageContentUpdated(request, campaign);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task On_tags_update_canvas_working()
        {
            var polling = new PollingList(InvocationContext);
            var request = new PollingEventRequest<DateMemory>
            {
                Memory = new DateMemory
                {
                    LastInteractionDate = new DateTime(2025, 5, 19, 13, 20, 11, DateTimeKind.Utc)
                }
            };
            var canvas = new PollingCanvasRequest
            {
                Tags = new List<string> { "translate-email", "email" }
            };

            var response = polling.OnCanvasTagAdded(request, canvas);
            var campaigntags = response.Result.Result.Canvases;
            foreach (var tags in campaigntags)
            {
                Console.WriteLine($"{tags.Name} - {tags.Id}");
            }

            Assert.IsNotNull(response);
        }


        [TestMethod]
        public async Task On_tags_update_email_template_working()
        {
            var polling = new PollingList(InvocationContext);
            var request = new PollingEventRequest<DateMemory>
            {
                Memory = new DateMemory
                {
                    LastInteractionDate = new DateTime(2025, 5, 19, 13, 20, 11, DateTimeKind.Utc)
                }
            };
            var campaign = new PollingEmailTemplateRequest
            {
                Tags = new List<string> { "translate-email", "email" }
            };

            var response = polling.OnEmailTemplateTagAdded(request, campaign);
            var templtaetags = response.Result.Result.Templates;
            foreach (var tags in templtaetags)
            {
                Console.WriteLine($"{tags.Name} - {tags.Id}");
            }

            Assert.IsNotNull(response);
        }


        [TestMethod]
        public async Task On_content_updated_working()
        {
            var polling = new ContentPollingList(InvocationContext);
            var request = new PollingEventRequest<DateMemory>
            {
                Memory = new DateMemory
                {
                    LastInteractionDate = DateTime.UtcNow.AddMonths(-3)
                }
            };
            var campaign = new PollingContentTypesOptionalFilter
            {
                ContentTypes = new List<string> { "campaign", "canvas"}
            };

            var response = await polling.OnContentCreatedOrUpdatedMultiple(request, campaign);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }
    }
}
