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
        if (string.IsNullOrEmpty(response.Content))
        {
            if (string.IsNullOrEmpty(response.ErrorMessage))
            {
                return new PluginApplicationException($"Unknown error occurred. Status code: {response.StatusCode}, {response.StatusDescription}");
            }
            
            return new PluginApplicationException(response.ErrorMessage);
        }

        if (response.ContentType?.Contains("html") == true || response.Content.TrimStart().StartsWith("<"))
        {
            var htmlErrorMessage = ExtractHtmlErrorMessage(response.Content);
            return new PluginApplicationException($"Expected JSON but received HTML ({response.StatusCode}). {htmlErrorMessage}");
        }

        var error = JsonConvert.DeserializeObject<ErrorOrMessageDto>(response.Content);
        string errorMessage;
        if (error?.Errors == null || !error.Errors.Any())
        {
            errorMessage = !string.IsNullOrWhiteSpace(error?.Message)
                ? error.Message
                : $"Unknown error occurred. Status code: {response.StatusCode}, Content: {response.Content}";
        }
        else
        {
            errorMessage = string.Join("; ", error.Errors.Select(e => e.Message));
        }
        
        return new PluginApplicationException(errorMessage);
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

    public override Task<RestResponse> ExecuteWithErrorHandling(RestRequest request)
    {
        int retryCount = 3;
        return ExecuteWithErrorHandlingAndRetries(request, retryCount);
    }

    public async Task<RestResponse> ExecuteWithErrorHandlingAndRetries(RestRequest request, int maxRetries)
    {
        int attempt = 0;
        Exception lastException = null;

        while (attempt < maxRetries)
        {
            try
            {
                RestResponse restResponse = await ExecuteAsync(request);
                if (!restResponse.IsSuccessStatusCode)
                {
                    if ((int)restResponse.StatusCode >= 400 && (int)restResponse.StatusCode < 500)
                    {
                        throw ConfigureErrorException(restResponse);
                    }
                    
                    if (attempt < maxRetries - 1)
                    {
                        lastException = ConfigureErrorException(restResponse);
                        attempt++;
                        
                        var delayMs = (int)Math.Pow(2, attempt) * 1000;
                        if (restResponse.Headers != null && restResponse.Headers.Any(h => h.Name != null && h.Name.Equals("Retry-After", StringComparison.OrdinalIgnoreCase)))
                        {
                            var retryAfterHeader = restResponse.Headers.First(h => h.Name != null && h.Name.Equals("Retry-After", StringComparison.OrdinalIgnoreCase));
                            if (int.TryParse(retryAfterHeader.Value?.ToString(), out int retryAfterSeconds))
                            {
                                delayMs = retryAfterSeconds * 1000;
                            }
                        }
                        
                        await Task.Delay(delayMs);
                        continue;
                    }
                    
                    throw ConfigureErrorException(restResponse);
                }
                
                return restResponse;
            }
            catch (PluginApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                lastException = ex;
                attempt++;
                
                if (attempt < maxRetries)
                {
                    var delayMs = (int)Math.Pow(2, attempt) * 1000;
                    await Task.Delay(delayMs);
                }
            }
        }
        
        throw lastException ?? new Exception("Request failed after retries with no exception captured");
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

            string? errorMessage = null;
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
