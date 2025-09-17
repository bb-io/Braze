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
        var request = new RestRequest("/campaigns/details");
        request.AddQueryParameter("campaign_id", input.CampaignId);
        return await Client.ExecuteWithErrorHandling<CampaignDto>(request);
    }

    [Action("Download campaign message", Description = "Download the campaign message in both JSON and HTML formats.")]
    public async Task<CampaignFileResponse> DownloadCampaignMessage([ActionParameter] CampaignMessageRequest input)
    {
        var mid = await ResolveMessageVariationIdAsync(input.CampaignId, input.MessageVariationId);

        var request = new RestRequest("/campaigns/translations", Method.Get);
        request.AddQueryParameter("campaign_id", input.CampaignId);
        request.AddQueryParameter("message_variation_id", mid);
        var result = await Client.ExecuteWithErrorHandling<TranslationsDto>(request);
        var localeVariant = result.Translations.FirstOrDefault(x => x.Locale.LocaleKey == input.Locale);
        if (localeVariant == null) throw new PluginMisconfigurationException($"The locale '{input.Locale}' is not present on this campaign message.");

        var identifier = new CampaignMessageIdentifier
        {
            CampaignId = input.CampaignId,
            MessageVariationId = mid
        };

        var jsonConverterService = ConverterFactory<CampaignMessageIdentifier>.CreateConverter(".json", fileManagementClient);
        var jsonFile = await jsonConverterService.ToFile(identifier, localeVariant.TranslationMap);

        var htmlConverterService = ConverterFactory<CampaignMessageIdentifier>.CreateConverter(".html", fileManagementClient);
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
        var fileExtension = Path.GetExtension(input.File.Name);

        var converter = ConverterFactory<CampaignMessageIdentifier>.CreateConverter(fileExtension, fileManagementClient);
        var (identifier, translationMap) = converter.FromFile(fileContent);

        var mid = await ResolveMessageVariationIdAsync(input.CampaignId, input.MessageVariationId ?? identifier?.MessageVariationId);

        var request = new RestRequest("/campaigns/translations");
        request.AddQueryParameter("campaign_id", input.CampaignId ?? identifier?.CampaignId);
        request.AddQueryParameter("message_variation_id", input.MessageVariationId ?? identifier?.MessageVariationId);
        var result = await Client.ExecuteWithErrorHandling<TranslationsDto>(request);
        var localeVariant = result.Translations.FirstOrDefault(x => x.Locale.LocaleKey == input.Locale);
        if (localeVariant == null) throw new PluginMisconfigurationException($"The locale '{input.Locale}' is not present on this campaign message.");


        var updateRequest = new RestRequest("/campaigns/translations", Method.Put);
        updateRequest.AddJsonBody(new 
        { 
            campaign_id = input.CampaignId ?? identifier?.CampaignId,
            message_variation_id = mid,
            locale_id = localeVariant.Locale.Uuid,
            translation_map = translationMap
        });

        await Client.ExecuteWithErrorHandling(updateRequest);
    }
}
