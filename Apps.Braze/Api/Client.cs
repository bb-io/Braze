using Apps.Braze.Constants;
using Apps.Braze.Dtos;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Braze.Api;

public class Client : BlackBirdRestClient
{
    public Client(IEnumerable<AuthenticationCredentialsProvider> creds) : base(new()
    {
        BaseUrl = new Uri(creds.Get(CredsNames.BaseUrl).Value),
    })
    {
        this.AddDefaultHeader("Authorization", $"Bearer {creds.Get(CredsNames.Key).Value}");
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        if (response.Content == null) return new PluginApplicationException(response.ErrorMessage);
        var error = JsonConvert.DeserializeObject<ErrorOrMessageDto>(response.Content);
        if (error.Errors == null || error.Errors.Count() == 0) return new PluginApplicationException(error.Message);
        return new PluginApplicationException(error.Errors.FirstOrDefault()?.Message);
    }
}
