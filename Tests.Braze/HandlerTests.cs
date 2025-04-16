using Apps.Braze.Actions;
using Apps.Braze.Handlers;
using Apps.Braze.Models.Campaigns;
using Apps.Braze.Models.Canvas;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Newtonsoft.Json;
using Tests.Braze.Base;

namespace Tests.Braze;

[TestClass]
public class HandlerTests : TestBase
{
    [TestMethod]
    public async Task Campaign_handler_works()
    {
        var handler = new CampaignDataHandler(InvocationContext);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.DisplayName}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

    [TestMethod]
    public async Task Email_handler_works()
    {
        var handler = new EmailTemplateDataHandler(InvocationContext);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.DisplayName}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

    [TestMethod]
    public async Task Canvas_handler_works()
    {
        var handler = new CanvasDataHandler(InvocationContext);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.DisplayName}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

    
}
