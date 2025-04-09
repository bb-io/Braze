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
                        new("https://rest.iad-01.braze.com", "US-01"),
                        new("https://rest.iad-02.braze.com", "US-02"),
                        new("https://rest.iad-03.braze.com", "US-03"),
                        new("https://rest.iad-04.braze.com", "US-04"),
                        new("https://rest.iad-05.braze.com", "US-05"),
                        new("https://rest.iad-06.braze.com", "US-06"),
                        new("https://rest.iad-07.braze.com", "US-07"),
                        new("https://rest.iad-08.braze.com", "US-08"),
                        new("https://rest.iad-10.braze.com", "US-10"),
                        new("https://rest.fra-01.braze.eu", "EU-01"),
                        new("https://rest.fra-02.braze.eu", "EU-02"),
                        new("https://rest.au-01.braze.com", "AU-01"),
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
