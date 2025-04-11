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
        var result = await actions.AddTranslationTagsToEmailTemplate(new EmailTemplateRequest { EmailTemplateId = "aec92c0b-f3ef-4bca-a61e-e42d96562334" });
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

}
