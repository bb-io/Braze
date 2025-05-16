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
            var request = new PollingEventRequest<TagMemory>
            {
                Memory = new TagMemory
                {
                    KnownTags = new List<string> { "tag1", "email_translate" }
                }
            };
            var campaign = new PollingCampaignRequest
            {
                CampaignId = "5ae9bab4-000d-4da4-8025-a8a7e3a227ba",
                Tags = new List<string> { "email_translate" }
            };

            var response = polling.OnCampaignTagAdded(request, campaign);
            var campaigntags = response.Result.Result.Tags;
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
            var request = new PollingEventRequest<TagMemory>
            {
                Memory = new TagMemory
                {
                    KnownTags = new List<string> { "tag1", "tag2" }
                }
            };
            var canvas = new PollingCanvasRequest
            {
                CanvasId = "762d1c20-2728-452e-985d-c49ebe31e3ae"
            };

            var response = polling.OnCanvasTagAdded(request, canvas);
            var campaigntags = response.Result.Result.Tags;
            foreach (var tags in campaigntags)
            {
                Console.WriteLine($"{tags}");
            }

            Assert.IsNotNull(response);
        }
    }
}
