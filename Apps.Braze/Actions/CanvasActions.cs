using Apps.Braze.Dtos;
using Apps.Braze.Models.Canvas;
using Apps.Braze.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;
using System.Text;

namespace Apps.Braze.Actions;

[ActionList("Canvas")]
public class CanvasActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
{
    [Action("Search canvases", Description = "Search through your canvases")]
    public async Task<CanvasListDto> SearchCanvas([ActionParameter] SearchCanvasRequest input)
    {
        var request = new RestRequest("/canvas/list");
        if (input.LastEdited.HasValue)
        {
            request.AddQueryParameter("last_edit.time[gt]", input.LastEdited.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
        }
        return await Client.ExecuteWithErrorHandling<CanvasListDto>(request);
    }

    [Action("Get canvas", Description = "Get all details of a specific canvas")]
    public async Task<CanvasDetailsDto> GetCanvas([ActionParameter] CanvasRequest input)
    {
        var request = new RestRequest("/canvas/details");
        request.AddQueryParameter("canvas_id", input.CanvasId);
        return await Client.ExecuteWithErrorHandling<CanvasDetailsDto>(request);
    }

    [Action("Download canvas message", Description = "Download the canvas message in both JSON and HTML format")]
    public async Task<CanvasFileResponse> DownloadCanvasMessage([ActionParameter] CanvasMessageRequest input)
    {
        var request = new RestRequest("/canvas/translations");
        request.AddQueryParameter("step_id", input.StepId);
        request.AddQueryParameter("workflow_id", input.CanvasId);
        request.AddQueryParameter("message_variation_id", input.MessageVariationId);
        var result = await Client.ExecuteWithErrorHandling<TranslationsDto>(request);

        var localeVariant = result.Translations.FirstOrDefault(x => x.Locale.LocaleKey == input.Locale);
        if (localeVariant == null) throw new PluginMisconfigurationException($"The locale '{input.Locale}' is not present on this canvas message.");

        var identifier = new CanvasMessageIdentifier
        {
            CanvasId = input.CanvasId,
            MessageVariationId = input.MessageVariationId,
            StepId = input.StepId
        };

        var jsonConverterService = ConverterFactory<CanvasMessageIdentifier>.CreateConverter(".json", fileManagementClient);
        var jsonFile = await jsonConverterService.ToFile(identifier, localeVariant.TranslationMap);

        var htmlConverterService = ConverterFactory<CanvasMessageIdentifier>.CreateConverter(".html", fileManagementClient);
        var htmlFile = await htmlConverterService.ToFile(identifier, localeVariant.TranslationMap);

        return new CanvasFileResponse
        {
            JsonFile = jsonFile,
            HtmlFile = htmlFile,
        };
    }

    [Action("Upload canvas message", Description = "Upload the canvas message content from a translated file")]
    public async Task UploadCanvasMessage([ActionParameter] UploadCanvasMessageRequest input)
    {
        var file = await fileManagementClient.DownloadAsync(input.File);
        var fileContent = Encoding.UTF8.GetString(await file.GetByteData());
        var fileExtension = Path.GetExtension(input.File.Name);

        var converter = ConverterFactory<CanvasMessageIdentifier>.CreateConverter(fileExtension, fileManagementClient);
        var (identifier, translationMap) = converter.FromFile(fileContent);

        var request = new RestRequest("/canvas/translations");
        request.AddQueryParameter("workflow_id", input.CanvasId ?? identifier?.CanvasId);
        request.AddQueryParameter("step_id", input.StepId);
        request.AddQueryParameter("message_variation_id", input.MessageVariationId ?? identifier?.MessageVariationId);
        var result = await Client.ExecuteWithErrorHandling<TranslationsDto>(request);
        var localeVariant = result.Translations.FirstOrDefault(x => x.Locale.LocaleKey == input.Locale);
        if (localeVariant == null) throw new PluginMisconfigurationException($"The locale '{input.Locale}' is not present on this canvas message.");


        var updateRequest = new RestRequest("/canvas/translations", Method.Put);
        updateRequest.AddJsonBody(new
        {
            workflow_id = input.CanvasId ?? identifier?.CanvasId,
            message_variation_id = input.MessageVariationId ?? identifier?.MessageVariationId,
            locale_id = localeVariant.Locale.Uuid,
            step_id = input.StepId,
            translation_map = translationMap
        });

        await Client.ExecuteWithErrorHandling(updateRequest);
    }
}