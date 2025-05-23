using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.Braze;

public class Application : IApplication, ICategoryProvider
{
    public IEnumerable<ApplicationCategory> Categories
    {
        get => [
            ApplicationCategory.Marketing
        ];
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}
