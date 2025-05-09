using Apps.Braze.Actions;
using Apps.Braze.Models.Campaigns;
using Apps.Braze.Models.EmailTemplates;
using Blackbird.Applications.Sdk.Common.Files;
using Newtonsoft.Json;
using System.Net.Mime;
using Tests.Braze.Base;

namespace Tests.Braze;

[TestClass]
public class EmailTemplateActionTests : TestBase
{
    [TestMethod]
    public async Task Add_translation_tags_works()
    {
        var actions = new EmailTemplateActions(InvocationContext, FileManager);
        await actions.AddTranslationTagsToEmailTemplate(new EmailTemplateRequest { EmailTemplateId = "aec92c0b-f3ef-4bca-a61e-e42d96562334" });
    }

    [TestMethod]
    public async Task Add_tags_works()
    {
        var actions = new EmailTemplateActions(InvocationContext, FileManager);
        await actions.AddTagsToEmailTemplate(
            new EmailTemplateRequest { EmailTemplateId = "8dc14a6c-0371-48de-ac18-efcf2ce0b7b5" },
            new EmailTemplateTagsRequest { Tags = ["email","translate-email"] });
    }

    [TestMethod]
    public async Task Remove_tags_works()
    {
        var actions = new EmailTemplateActions(InvocationContext, FileManager);
        await actions.RemoveTagsFromEmailTemplate(
            new EmailTemplateRequest { EmailTemplateId = "8dc14a6c-0371-48de-ac18-efcf2ce0b7b5" },
            new EmailTemplateTagsRequest { Tags = ["email"] });
    }

    [TestMethod]
    public async Task Search_email_templates_works()
    {
        var actions = new EmailTemplateActions(InvocationContext, FileManager);
        var result = await actions.SearchEmailTemplates(new SearchEmailTemplatesRequest { });

        Console.WriteLine($"Total: {result.Count}");
        foreach (var item in result.Templates)
        {
            Console.WriteLine($"{item.Id}: {item.Name}");
        }

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task Get_email_templates_works()
    {
        var actions = new EmailTemplateActions(InvocationContext, FileManager);
        var response = await actions.GetEmailTemplate(new EmailTemplateRequest { EmailTemplateId = "8dc14a6c-0371-48de-ac18-efcf2ce0b7b5" });
        var json = JsonConvert.SerializeObject(response, Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsNotNull(actions);
    }

    [TestMethod]
    public async Task Create_email_template_works()
    {
        var actions = new EmailTemplateActions(InvocationContext, FileManager);
        var response = await actions.CreateEmailTemplate(
            new CreateEmailTemplateRequest {TemplateName="Test", Subject="test", HtmlBody=new FileReference { Name="test.html"} });
        var json = JsonConvert.SerializeObject(response, Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsNotNull(actions);
    }

    [TestMethod]
    public async Task Update_email_template_works()
    {
        var actions = new EmailTemplateActions(InvocationContext, FileManager);
        await actions.UpdateEmailTemplate(
            new UpdateEmailTemplateRequest {EmailTemplateId= "30470433-f96b-42f9-a2db-4d31711f9117", HtmlBody = new FileReference { Name = "test.html" } });
        Assert.IsTrue(true);
    }
}
