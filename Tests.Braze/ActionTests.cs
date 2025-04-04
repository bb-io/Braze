using Apps.Braze.Actions;
using Tests.Braze.Base;

namespace Tests.Braze;

[TestClass]
public class ActionTests : TestBase
{
    [TestMethod]
    public async Task Dynamic_handler_works()
    {
        var actions = new Actions(InvocationContext);

        await actions.Action();
    }
}
