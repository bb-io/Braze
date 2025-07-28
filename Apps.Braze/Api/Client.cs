using Apps.Braze.Constants;
using Apps.Braze.Dtos;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;
using System.Text.RegularExpressions;

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

        if (response.ContentType?.Contains("html") == true || response.Content.TrimStart().StartsWith("<"))
        {
            var htmlErrorMessage = ExtractHtmlErrorMessage(response.Content);
            return new PluginApplicationException($"Expected JSON but received HTML response. {htmlErrorMessage}");
        }

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

    private string ExtractHtmlErrorMessage(string htmlContent)
    {
        if (string.IsNullOrWhiteSpace(htmlContent))
            return "Empty HTML response received.";

        try
        {
            var titleMatch = Regex.Match(htmlContent, @"<title[^>]*>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var title = titleMatch.Success ? titleMatch.Groups[1].Value.Trim() : null;

            var h1Match = Regex.Match(htmlContent, @"<h1[^>]*>(.*?)</h1>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var h1 = h1Match.Success ? StripHtmlTags(h1Match.Groups[1].Value).Trim() : null;

            var errorPatterns = new[]
            {
                @"<[^>]*class[^>]*error[^>]*>(.*?)</[^>]*>",
                @"<[^>]*class[^>]*message[^>]*>(.*?)</[^>]*>",
                @"<p[^>]*>(.*?)</p>",
                @"<div[^>]*>(.*?)</div>"
            };

            string errorMessage = null;
            foreach (var pattern in errorPatterns)
            {
                var match = Regex.Match(htmlContent, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (match.Success && !string.IsNullOrWhiteSpace(match.Groups[1].Value))
                {
                    errorMessage = StripHtmlTags(match.Groups[1].Value).Trim();
                    if (errorMessage.Length > 10)
                        break;
                }
            }

            var messageParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(title) && !title.Contains("DOCTYPE") && !title.Contains("html"))
                messageParts.Add($"Page title: {title}");

            if (!string.IsNullOrWhiteSpace(h1) && h1 != title)
                messageParts.Add($"Error: {h1}");

            if (!string.IsNullOrWhiteSpace(errorMessage) && errorMessage != title && errorMessage != h1)
                messageParts.Add($"Details: {errorMessage}");

            if (messageParts.Any())
                return string.Join(" | ", messageParts);

            var cleanContent = StripHtmlTags(htmlContent).Trim();
            if (cleanContent.Length > 200)
                cleanContent = cleanContent.Substring(0, 200) + "...";

            return string.IsNullOrWhiteSpace(cleanContent)
                ? "Received HTML response without readable content."
                : $"HTML content: {cleanContent}";
        }
        catch (Exception ex)
        {
            return $"Could not parse HTML response. Error: {ex.Message}";
        }
    }

    private string StripHtmlTags(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        var withoutTags = Regex.Replace(html, @"<[^>]*>", " ");
        withoutTags = System.Net.WebUtility.HtmlDecode(withoutTags);
        withoutTags = Regex.Replace(withoutTags, @"\s+", " ");
        return withoutTags.Trim();
    }
}
