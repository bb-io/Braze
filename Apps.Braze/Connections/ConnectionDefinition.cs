using Apps.Braze.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Braze.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>
    {
        new()
        {
            Name = "API key",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredsNames.BaseUrl) 
                {
                    DisplayName = "Instance", 
                    DataItems = new ConnectionPropertyValue[]
                    { 
                        new("US-01", "https://rest.iad-01.braze.com"),
                        new("US-02", "https://rest.iad-02.braze.com"),
                        new("US-03", "https://rest.iad-03.braze.com"),
                        new("US-04", "https://rest.iad-04.braze.com"),
                        new("US-05", "https://rest.iad-05.braze.com"),
                        new("US-06", "https://rest.iad-06.braze.com"),
                        new("US-07", "https://rest.iad-07.braze.com"),
                        new("US-08", "https://rest.iad-08.braze.com"),
                        new("US-10", "https://rest.iad-10.braze.com"),
                        new("EU-01", "https://rest.fra-01.braze.eu"),
                        new("EU-02", "https://rest.fra-02.braze.eu"),
                        new("AU-01", "https://rest.au-01.braze.com"),
                    } 
                },
                new(CredsNames.Key) { DisplayName = "API Key", Sensitive = true}
            }
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values) => values.Select(x => new AuthenticationCredentialsProvider(x.Key, x.Value)
        ).ToList();
}
