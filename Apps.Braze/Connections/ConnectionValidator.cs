using Apps.Braze.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Braze.Connections;

public class ConnectionValidator(InvocationContext invocationContext) : BaseInvocable(invocationContext), IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = new Client(authenticationCredentialsProviders);
            
            var request = new RestRequest("/campaigns/list");
            await client.ExecuteWithErrorHandlingAndRetries(request, 5);
            return new()
            {
                IsValid = true
            };
        } 
        catch(Exception ex)
        {
            InvocationContext.Logger?.LogError($"[BrazeConnectionValidator] Connection validation failed: {ex.Message}", []);
            return new()
            {
                IsValid = false,
                Message = ex.Message
            };
        }

    }
}
