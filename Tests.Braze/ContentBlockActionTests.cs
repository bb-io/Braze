using Apps.Braze.Actions;
using Apps.Braze.Models.ContentBlocks;
using Tests.Braze.Base;

namespace Tests.Braze;

[TestClass]
public class ContentBlockActionTests : TestBase
{
    [TestMethod]
    public async Task GetContentBlock_IsSuccess()
    {
        var action = new ContentBlockActions(InvocationContext, FileManager);

        var result = await action.GetContentBlock(new()
        {
            ContentBlockId = "1b5055c3-790f-47a6-93af-31561dee68b0",
        });

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);

        Console.WriteLine(json);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task SearchContentBlock_IsSuccess()
    {
        var action = new ContentBlockActions(InvocationContext, FileManager);

        var result = await action.SearchContentBlocks(new()
        {
        });

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);

        Console.WriteLine(json);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task DownloadContentBlock_IsSuccess()
    {
        var action = new ContentBlockActions(InvocationContext, FileManager);

        var result = await action.DownloadContentBlock(new Apps.Braze.Models.ContentBlocks.ContentBlockRequest
        {
            ContentBlockId = "1b5055c3-790f-47a6-93af-31561dee68b0",
        });

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);

        Console.WriteLine(json);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UploadContentBlock_IsSuccess()
    {
        var action = new ContentBlockActions(InvocationContext, FileManager);

        await action.UploadContentBlock(new UploadContentBlockRequest
        {
            ContentBlockId = "1b5055c3-790f-47a6-93af-31561dee68b0",
            Content = new Blackbird.Applications.Sdk.Common.Files.FileReference { Name = "1b5055c3-790f-47a6-93af-31561dee68b0.html" }
        });
        Assert.IsTrue(true);
    }
}

