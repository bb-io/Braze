using Apps.Braze.Actions;
using Apps.Braze.Models.Campaigns;
using Apps.Braze.Models.Canvas;
using Blackbird.Applications.Sdk.Common.Files;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Tests.Braze.Base;

namespace Tests.Braze
{
    [TestClass]
    public class CanvasActionTests : TestBase
    {
        [TestMethod]
        public async Task Search_canvas_works()
        {
            var actions = new CanvasActions(InvocationContext, FileManager);
            var result = await actions.SearchCanvas(new SearchCanvasRequest { });
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Search_canvases_with_last_edited_works()
        {
            var actions = new CanvasActions(InvocationContext, FileManager);
            var result = await actions.SearchCanvas(new SearchCanvasRequest { LastEdited = new DateTime(2025, 4, 6) });
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Get_canvas_details_works()
        {
            var actions = new CanvasActions(InvocationContext, FileManager);
            var result = await actions.GetCanvas(new CanvasRequest { CanvasId= "762d1c20-2728-452e-985d-c49ebe31e3ae" });
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Download_canvas_message_works()
        {
            var actions = new CanvasActions(InvocationContext, FileManager);
            var result = await actions.DownloadCanvasMessage(new CanvasMessageRequest
            {
                StepId = "1583d01b-b953-42f3-a47e-552d51aad77d",
                MessageVariationId = "614cb013-ff1d-40cd-85c2-94676f19b814",
                Locale = "en",
                CanvasId = "762d1c20-2728-452e-985d-c49ebe31e3ae"
            });
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }

        [TestMethod]
        public async Task Upload_canvas_json_works()
        {
            var actions = new CanvasActions(InvocationContext, FileManager);
            var file = new FileReference() { Name = "614cb013-ff1d-40cd-85c2-94676f19b814.json", ContentType = MediaTypeNames.Application.Json };
            await actions.UploadCanvasMessage(new UploadCanvasMessageRequest
            {
                Locale = "en",
                StepId = "1583d01b-b953-42f3-a47e-552d51aad77d",
                CanvasId = "762d1c20-2728-452e-985d-c49ebe31e3ae",
                File = file,
            });
        }

        [TestMethod]
        public async Task Upload_canvas_html_works()
        {
            var actions = new CanvasActions(InvocationContext, FileManager);
            var file = new FileReference() { Name = "614cb013-ff1d-40cd-85c2-94676f19b814.html", ContentType = MediaTypeNames.Text.Html };
            await actions.UploadCanvasMessage(new UploadCanvasMessageRequest
            {
                Locale = "en",
                StepId = "1583d01b-b953-42f3-a47e-552d51aad77d",
                CanvasId = "762d1c20-2728-452e-985d-c49ebe31e3ae",
                File = file,
            });
        }

    }
}
