using Apps.Braze.Polling;
using Apps.Braze.Polling.Memory;
using Blackbird.Applications.Sdk.Common.Polling;
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
                    LastInteractionDate = new DateTime(2025, 5, 19, 13, 20, 11, DateTimeKind.Utc)
                }
            };
            var campaign = new PollingCampaignRequest
            {
                //CampaignId= "841280d5-3693-4ff1-b32b-039dd4407670"
                Tags = new List<string> { "translate-email", "email" }
            };

            var response = polling.OnCampaignTagAdded(request, campaign);
            var campaigntags = response.Result.Result.Campaigns;
            foreach (var tags in campaigntags)
            {
                Console.WriteLine($"{tags}");
            }

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
    }
}
