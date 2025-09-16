using Apps.Braze.Actions;
using Apps.Braze.Models.Content;
using Tests.Braze.Base;

namespace Tests.Braze
{
    [TestClass]
    public class ContentActionTests : TestBase
    {
        [TestMethod]
        public async Task SearchContent_IsSuccess()
        {
            var action = new ContentActions(InvocationContext, FileManager);

            var result = await action.SearchContent(new()
            {
                ContentType = "canvas",
                EditedAfter = DateTime.UtcNow.AddYears(-1)
            });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);

            Console.WriteLine(json);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task DownloadContent_IsSuccess()
        {
            var action = new ContentActions(InvocationContext, FileManager);

            var result = await action.DownloadContent(new DownloadContentRequest
            {
                ContentType = "email_template",
                Locale = "en",
                ContentId = "f26f01ae-628a-4781-a55e-76a07373bde0",
                //StepId = "1583d01b-b953-42f3-a47e-552d51aad77d",
                //MessageVariationId = "614cb013-ff1d-40cd-85c2-94676f19b814",
            });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);

            Console.WriteLine(json);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task UploadContent_IsSuccess()
        {
            var action = new ContentActions(InvocationContext, FileManager);

            await action.UploadContent(new UploadContentRequest
            {
                ContentType = "canvas",
                //Locale = "en",
                ContentId = "58a14503-9626-4e51-8a11-3565cd64eefa",
                //StepId = "1583d01b-b953-42f3-a47e-552d51aad77d",
                //MessageVariationId = "614cb013-ff1d-40cd-85c2-94676f19b814",
                Content= new Blackbird.Applications.Sdk.Common.Files.FileReference { Name= "614cb013-ff1d-40cd-85c2-94676f19b814.html" }
            });
            Assert.IsTrue(true);
        }
    }
}
