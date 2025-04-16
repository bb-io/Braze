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
        MaxTimeout = 120000
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

    public override async Task<T> ExecuteWithErrorHandling<T>(RestRequest request)
    {
        string content = (await ExecuteWithErrorHandling(request)).Content;
        T val = JsonConvert.DeserializeObject<T>(content, JsonSettings);
        if (val == null)
        {
            throw new Exception($"Could not parse {content} to {typeof(T)}");
        }

        return val;
    }

    public override async Task<RestResponse> ExecuteWithErrorHandling(RestRequest request)
    {
        int retryCount = 3;
        int attempt = 0;
        Exception lastException = null;

        while (attempt < retryCount)
        {
            try
            {
                RestResponse restResponse = await ExecuteAsync(request);
                if (!restResponse.IsSuccessStatusCode)
                {
                    throw ConfigureErrorException(restResponse);
                }
                return restResponse;
            }
            catch (Exception ex)
            {
                lastException = ex;
                attempt++;

            }
        }
        throw lastException;
    }
}
