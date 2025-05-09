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
        var actions = new EmailTemplateActions(InvocationContext);
        await actions.AddTranslationTagsToEmailTemplate(new EmailTemplateRequest { EmailTemplateId = "aec92c0b-f3ef-4bca-a61e-e42d96562334" });
    }

    [TestMethod]
    public async Task Add_tags_works()
    {
        var actions = new EmailTemplateActions(InvocationContext);
        await actions.AddTagsToEmailTemplate(
            new EmailTemplateRequest { EmailTemplateId = "8dc14a6c-0371-48de-ac18-efcf2ce0b7b5" },
            new EmailTemplateTagsRequest { Tags = ["email","translate-email"] });
    }

}
