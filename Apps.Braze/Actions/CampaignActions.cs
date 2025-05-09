using Apps.Braze.Constants;
using Apps.Braze.Dtos;
using Apps.Braze.Models.Campaigns;
using Apps.Braze.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Mime;
using System.Text;

namespace Apps.Braze.Actions;

[ActionList]
public class CampaignActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
{
    [Action("Search campaigns", Description = "Search through your campaigns")]
    public async Task<CampaignListDto> SearchCampaigns([ActionParameter] SearchCampaignsRequest input)
    {
        var request = new RestRequest("/campaigns/list");
        if (input.LastEdited.HasValue)
        {
            request.AddQueryParameter("last_edit.time[gt]", input.LastEdited.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
        }
        return await Client.ExecuteWithErrorHandling<CampaignListDto>(request);
    }

    [Action("Get campaign", Description = "Get all details of a specific campaign")]
    public async Task<CampaignDto> GetCampaign([ActionParameter] CampaignRequest input)
    {
        var cid = await ResolveCampaignIdAsync(input.CampaignId);
        var request = new RestRequest("/campaigns/details");
        request.AddQueryParameter("campaign_id", cid);
        return await Client.ExecuteWithErrorHandling<CampaignDto>(request);
    }

    [Action("Download campaign message", Description = "Download the campaign message in both JSON and HTML formats.")]
    public async Task<CampaignFileResponse> DownloadCampaignMessage([ActionParameter] CampaignMessageRequest input)
    {
        var cid = await ResolveCampaignIdAsync(input.CampaignId);
        var mid = await ResolveMessageVariationIdAsync(cid, input.MessageVariationId);

        var request = new RestRequest("/campaigns/translations", Method.Get);
        request.AddQueryParameter("campaign_id", cid);
        request.AddQueryParameter("message_variation_id", mid);
        var result = await Client.ExecuteWithErrorHandling<TranslationsDto>(request);
        var localeVariant = result.Translations.FirstOrDefault(x => x.Locale.LocaleKey == input.Locale);
        if (localeVariant == null) throw new PluginMisconfigurationException($"The locale '{input.Locale}' is not present on this campaign message.");

        var identifier = new CampaignMessageIdentifier
        {
            CampaignId = cid,
            MessageVariationId = mid
        };

        var jsonConverterService = ConverterFactory<CampaignMessageIdentifier>.CreateConverter(MediaTypeNames.Application.Json, fileManagementClient);
        var jsonFile = await jsonConverterService.ToFile(identifier, localeVariant.TranslationMap);

        var htmlConverterService = ConverterFactory<CampaignMessageIdentifier>.CreateConverter(MediaTypeNames.Text.Html, fileManagementClient);
        var htmlFile = await htmlConverterService.ToFile(identifier, localeVariant.TranslationMap);

        return new CampaignFileResponse
        {
            JsonFile = jsonFile,
            HtmlFile = htmlFile,
        };
    }

    [Action("Upload campaign message", Description = "Upload the campaign message content from a translated file")]
    public async Task UploadCampaignMessage([ActionParameter] UploadCampaignMessageRequest input)
    {
        var file = await fileManagementClient.DownloadAsync(input.File);
        var fileContent = Encoding.UTF8.GetString(await file.GetByteData());

        var converter = ConverterFactory<CampaignMessageIdentifier>.CreateConverter(input.File.ContentType, fileManagementClient);
        var (identifier, translationMap) = converter.FromFile(fileContent);

        var cid = await ResolveCampaignIdAsync(input.CampaignId ?? identifier?.CampaignId);
        var mid = await ResolveMessageVariationIdAsync(cid, input.MessageVariationId ?? identifier?.MessageVariationId);

        var request = new RestRequest("/campaigns/translations");
        request.AddQueryParameter("campaign_id", cid);
        request.AddQueryParameter("message_variation_id", mid);
        var result = await Client.ExecuteWithErrorHandling<TranslationsDto>(request);
        var localeVariant = result.Translations.FirstOrDefault(x => x.Locale.LocaleKey == input.Locale);
        if (localeVariant == null) throw new PluginMisconfigurationException($"The locale '{input.Locale}' is not present on this campaign message.");


        var updateRequest = new RestRequest("/campaigns/translations", Method.Put);
        updateRequest.AddJsonBody(new 
        { 
            campaign_id = cid,
            message_variation_id = mid,
            locale_id = localeVariant.Locale.Uuid,
            translation_map = translationMap
        });

        await Client.ExecuteWithErrorHandling(updateRequest);
    }


    private async Task<string> ResolveCampaignIdAsync(string? campaignId)
    {
        if (!string.IsNullOrWhiteSpace(campaignId))
            return campaignId!;

        var listRequest = new RestRequest("/campaigns/list", Method.Get);
        var listDto = await Client.ExecuteWithErrorHandling<CampaignListDto>(listRequest);
        var first = listDto.Campaigns.FirstOrDefault();
        if (first == null)
            throw new PluginApplicationException("No campaigns found to default to.");
        return first.Id;
    }

    private async Task<string> ResolveMessageVariationIdAsync(string campaignId, string? messageVariationId)
    {
        if (!string.IsNullOrWhiteSpace(messageVariationId))
            return messageVariationId!;

        var detailsReq = new RestRequest("/campaigns/details", Method.Get);
        detailsReq.AddQueryParameter("campaign_id", campaignId);
        var campaign = await Client.ExecuteWithErrorHandling<CampaignDto>(detailsReq);

        var first = campaign.MessageVariations.FirstOrDefault();
        if (first == null)
            throw new PluginApplicationException($"No message variations found for campaign '{campaignId}'.");
        return first.Id;
    }
}
