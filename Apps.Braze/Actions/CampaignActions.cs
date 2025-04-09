using Apps.Braze.Constants;
using Apps.Braze.Dtos;
using Apps.Braze.Models.Campaigns;
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
        var request = new RestRequest("/campaigns/translations");
        request.AddQueryParameter("campaign_id", input.CampaignId);
        request.AddQueryParameter("message_variation_id", input.MessageVariationId);
        var result = await Client.ExecuteWithErrorHandling<TranslationsDto>(request);
        var localeVariant = result.Translations.FirstOrDefault(x => x.Locale.LocaleKey == input.Locale);
        if (localeVariant == null) throw new PluginMisconfigurationException($"The locale '{input.Locale}' is not present on this campaign message.");

        var outputModel = new CampaignMessageJson
        {
            CampaignId = input.CampaignId,
            MessageVariantId = input.MessageVariationId,            
            TranslationMap = localeVariant.TranslationMap
        };

        var outputModelJson = JsonConvert.SerializeObject(outputModel, Formatting.Indented);
        var jsonFile = await fileManagementClient.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes(outputModelJson)), MediaTypeNames.Application.Json, $"{input.MessageVariationId}.json");

        var (doc, bodyNode) = PrepareEmptyHtmlDocument(input.CampaignId, input.MessageVariationId);
        foreach (var (key, value) in localeVariant.TranslationMap)
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.InnerHtml = value;
            node.SetAttributeValue(ConvertConstants.TranslationKeyAttribute, key);
            bodyNode.AppendChild(node);
        }

        var htmlFile = await fileManagementClient.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes(doc.DocumentNode.OuterHtml)),
            MediaTypeNames.Text.Html, $"{input.MessageVariationId}.html");

        return new CampaignFileResponse
        {
            JsonFile = jsonFile,
            HtmlFile = htmlFile,
        };
    }

    [Action("Upload campaign message", Description = "Upload the campaign message content from a translated file")]
    public async Task UploadCampaignMessage([ActionParameter] UploadCampaignMessageRequest input)
    {
        string campaignId;
        string messageVariationId;
        Dictionary<string, string> translationMap;

        var file = await fileManagementClient.DownloadAsync(input.File);
        var fileContent = Encoding.UTF8.GetString(await file.GetByteData());

        if (input.File.ContentType == MediaTypeNames.Application.Json)
        {
            var json = JsonConvert.DeserializeObject<CampaignMessageJson>(fileContent);
            campaignId = json.CampaignId;
            messageVariationId = json.MessageVariantId;
            translationMap = json.TranslationMap;
        }
        else if (input.File.ContentType == MediaTypeNames.Text.Html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(fileContent);

            campaignId = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-campaign-id']")?.GetAttributeValue("content", null) ?? "";
            messageVariationId = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-message-id']")?.GetAttributeValue("content", null) ?? "";

            translationMap = doc.DocumentNode.Descendants()
            .Where(x => x.Attributes[ConvertConstants.TranslationKeyAttribute] is not null)
            .ToDictionary(x => x.Attributes[ConvertConstants.TranslationKeyAttribute].Value, x => x.InnerHtml);
        }
        else throw new PluginMisconfigurationException($"The file type '{input.File.ContentType}' is not supported. Use a transformed file that was exported from the 'Download' action.");

        var request = new RestRequest("/campaigns/translations");
        request.AddQueryParameter("campaign_id", input.CampaignId ?? campaignId);
        request.AddQueryParameter("message_variation_id", input.MessageVariationId ?? messageVariationId);
        var result = await Client.ExecuteWithErrorHandling<TranslationsDto>(request);
        var localeVariant = result.Translations.FirstOrDefault(x => x.Locale.LocaleKey == input.Locale);
        if (localeVariant == null) throw new PluginMisconfigurationException($"The locale '{input.Locale}' is not present on this campaign message.");


        var updateRequest = new RestRequest("/campaigns/translations", Method.Put);
        updateRequest.AddJsonBody(new 
        { 
            campaign_id = input.CampaignId ?? campaignId,
            message_variation_id = input.MessageVariationId ?? messageVariationId,
            locale_id = localeVariant.Locale.Uuid,
            translation_map = translationMap
        });

        await Client.ExecuteWithErrorHandling(updateRequest);
    }

    private (HtmlDocument document, HtmlNode bodyNode) PrepareEmptyHtmlDocument(string campaignId, string messageVariantId)
    {
        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement(HtmlConstants.Html);
        htmlDoc.DocumentNode.AppendChild(htmlNode);

        var headNode = htmlDoc.CreateElement(HtmlConstants.Head);
        htmlNode.AppendChild(headNode);

        var campaignMetaNode = htmlDoc.CreateElement("meta");
        campaignMetaNode.SetAttributeValue("name", "blackbird-campaign-id");
        campaignMetaNode.SetAttributeValue("content", campaignId);
        headNode.AppendChild(campaignMetaNode);

        var messageMetaNode = htmlDoc.CreateElement("meta");
        messageMetaNode.SetAttributeValue("name", "blackbird-message-id");
        messageMetaNode.SetAttributeValue("content", messageVariantId);
        headNode.AppendChild(messageMetaNode);

        var bodyNode = htmlDoc.CreateElement(HtmlConstants.Body);
        htmlNode.AppendChild(bodyNode);

        return (htmlDoc, bodyNode);
    }
}
