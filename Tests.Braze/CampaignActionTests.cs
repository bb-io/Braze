using Apps.Braze.Actions;
using Apps.Braze.Models.Campaigns;
using Blackbird.Applications.Sdk.Common.Files;
using Newtonsoft.Json;
using System.Net.Mime;
using Tests.Braze.Base;

namespace Tests.Braze;

[TestClass]
public class CampaignActionTests : TestBase
{
    [TestMethod]
    public async Task Search_campaigns_works()
    {
        var actions = new CampaignActions(InvocationContext, FileManager);
        var result = await actions.SearchCampaigns(new SearchCampaignsRequest { });
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.IsTrue(result.Campaigns.Count() > 0);
    }

    [TestMethod]
    public async Task Search_campaigns_with_last_edited_works()
    {
        var actions = new CampaignActions(InvocationContext, FileManager);
        var result = await actions.SearchCampaigns(new SearchCampaignsRequest { LastEdited = new DateTime(2025, 4, 6) });
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.IsTrue(result.Campaigns.Count() > 0);
    }

    [TestMethod]
    public async Task Get_campaign_works()
    {
        var actions = new CampaignActions(InvocationContext, FileManager);
        var result = await actions.GetCampaign(new CampaignRequest { CampaignId = "7d41c34e-0669-45e5-9ad1-28c80af8a2d7" });
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.IsTrue(result.MessageVariations.Count() > 0);
    }

    [TestMethod]
    public async Task Download_campaign_message_works()
    {
        var actions = new CampaignActions(InvocationContext, FileManager);
        var result = await actions.DownloadCampaignMessage(new CampaignMessageRequest 
        {
            CampaignId = "80fa4d32-60fa-497e-a0e4-dcc2db212baf",
            //MessageVariationId = "c4b245d3-446a-4e9e-8584-dde418305461",
            Locale = "en",
        });
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task Upload_campaign_json_works()
    {
        var actions = new CampaignActions(InvocationContext, FileManager);
        var file = new FileReference() { Name = "c4b245d3-446a-4e9e-8584-dde418305461.json", ContentType = MediaTypeNames.Application.Json };
        await actions.UploadCampaignMessage(new UploadCampaignMessageRequest
        {
            Locale = "de",
            File = file,
        });
    }

    [TestMethod]
    public async Task Upload_campaign_html_works()
    {
        var actions = new CampaignActions(InvocationContext, FileManager);
        var file = new FileReference() { Name = "5200cf84-f2c2-47fc-84d7-f2094dd6f48e.html", ContentType = MediaTypeNames.Text.Html };
        await actions.UploadCampaignMessage(new UploadCampaignMessageRequest
        {
            Locale = "de",
            File = file,
        });
    }
}
