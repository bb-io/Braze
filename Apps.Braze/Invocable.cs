using Apps.Braze.Api;
using Apps.Braze.Dtos;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Braze;

public class Invocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected Client Client { get; }
    public Invocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new(Creds);
    }

    protected async Task<string> ResolveMessageVariationIdAsync(string campaignId, string? messageVariationId)
    {
        if (!string.IsNullOrWhiteSpace(messageVariationId))
            return messageVariationId!;

        var detailsReq = new RestRequest("/campaigns/details", Method.Get)
            .AddQueryParameter("campaign_id", campaignId);

        var campaign = await Client.ExecuteWithErrorHandling<CampaignDto>(detailsReq);

        var first = campaign.MessageVariations.FirstOrDefault();
        if (first == null)
            throw new PluginApplicationException($"No message variations found for campaign '{campaignId}'.");

        return first.Id;
    }

}
