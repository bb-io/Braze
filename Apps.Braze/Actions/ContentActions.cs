using Apps.Braze.Dtos;
using Apps.Braze.Models.Campaigns;
using Apps.Braze.Models.Canvas;
using Apps.Braze.Models.Content;
using Apps.Braze.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff2;
using Newtonsoft.Json;
using RestSharp;
using System.Text;

namespace Apps.Braze.Actions
{
    [ActionList("Content")]
    public class ContentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
    {
        [BlueprintActionDefinition(BlueprintAction.SearchContent)]
        [Action("Search content")]
        public async Task<SearchContentResponse> SearchContent([ActionParameter] SearchContentRequest input)
        {
            if (string.IsNullOrEmpty(input.ContentType))
            {
                throw new PluginMisconfigurationException("Content type must be provided. Please check your input and try again");
            }

            var tasks = new List<Task<IEnumerable<ContentItem>>>();

            if (input.ContentType.Contains("campaign"))
                tasks.Add(FetchCampaigns(input.EditedAfter));


            if (input.ContentType.Contains("canvas"))
                tasks.Add(FetchCanvases(input.EditedAfter));


            if (input.ContentType.Contains("email_template"))
                tasks.Add(FetchEmailTemplates(input.EditedAfter, input.EditedBefore, input.Limit, input.Offset));

            var results = await Task.WhenAll(tasks);
            var items = results.SelectMany(x => x ?? Enumerable.Empty<ContentItem>()).ToList();

            items = items
                .OrderByDescending(i => i.LastEdited ?? i.CreatedAt ?? DateTime.MinValue)
                .ToList();

            return new SearchContentResponse { Items = items };
        }


        [BlueprintActionDefinition(BlueprintAction.DownloadContent)]
        [Action("Download content", Description = "Download Braze content (campaign or canvas) as JSON and HTML.")]
        public async Task<DownloadContentResponse> DownloadContent([ActionParameter] DownloadContentRequest input)
        {
            if (string.IsNullOrWhiteSpace(input.ContentType))
                throw new PluginMisconfigurationException("Content type is required (campaign | canvas).");


            var type = input.ContentType.Trim().ToLowerInvariant();
            return type switch
            {
                "campaign" => await DownloadCampaignAsync(input),
                "canvas" => await DownloadCanvasAsync(input),
                "email_template" => await DownloadEmailTemplateAsync(input),
                _ => throw new PluginMisconfigurationException(
                    "Unsupported content type. Valid: campaign | canvas | email_template.")
            };
        }

        [BlueprintActionDefinition(BlueprintAction.UploadContent)]
        [Action("Upload content", Description = "Upload Braze content (campaign, canvas, or email_template) from a translated file.")]
        public async Task UploadContent([ActionParameter] UploadContentRequest input)
        {
            if (string.IsNullOrWhiteSpace(input.ContentType))
                throw new PluginMisconfigurationException("Content type is required (campaign | canvas | email_template).");

            var type = input.ContentType.Trim().ToLowerInvariant();
            switch (type)
            {
                case "campaign":
                    await UploadCampaignAsync(input);
                    break;
                case "canvas":
                    await UploadCanvasAsync(input);
                    break;
                case "email_template":
                    await UploadEmailTemplateAsync(input);
                    break;
                default:
                    throw new PluginMisconfigurationException("Unsupported content type. Valid: campaign | canvas | email_template.");
            }
        }


        public async Task UploadCampaignAsync(UploadContentRequest input)
        {
            if (input.Content is null)
                throw new PluginMisconfigurationException("Content file is required.");
            if (string.IsNullOrWhiteSpace(input.Locale))
                throw new PluginMisconfigurationException("Locale is required.");

            var (raw, ext) = await ReadContentTextAsync(input.Content);

            var conv = ConverterFactory<CampaignMessageIdentifier>.CreateConverter(ext, fileManagementClient);
            var (identifier, translationMap) = conv.FromFile(raw);

            var campaignId = input.ContentId ?? identifier?.CampaignId
                ?? throw new PluginMisconfigurationException("Campaign ID is missing and could not be inferred from the file.");

            var mid = await ResolveMessageVariationIdAsync(campaignId, input.MessageVariationId ?? identifier?.MessageVariationId);

            var getReq = new RestRequest("/campaigns/translations", Method.Get)
                .AddQueryParameter("campaign_id", campaignId)
                .AddQueryParameter("message_variation_id", mid);
            var getRes = await Client.ExecuteWithErrorHandling<TranslationsDto>(getReq);

            var localeVariant = getRes?.Translations?.FirstOrDefault(x => x.Locale.LocaleKey == input.Locale);
            if (localeVariant == null)
                throw new PluginMisconfigurationException($"The locale '{input.Locale}' is not present on this campaign message.");

            var putReq = new RestRequest("/campaigns/translations", Method.Put);
            putReq.AddJsonBody(new
            {
                campaign_id = campaignId,
                message_variation_id = mid,
                locale_id = localeVariant.Locale.Uuid,
                translation_map = translationMap
            });

            await Client.ExecuteWithErrorHandling(putReq);
        }

        public async Task UploadCanvasAsync(UploadContentRequest input)
        {
            if (input.Content is null)
                throw new PluginMisconfigurationException("Content file is required.");
            if (string.IsNullOrWhiteSpace(input.Locale))
                throw new PluginMisconfigurationException("Locale is required.");

            var (raw, ext) = await ReadContentTextAsync(input.Content);

            var conv = ConverterFactory<CanvasMessageIdentifier>.CreateConverter(ext, fileManagementClient);
            var (identifier, translationMap) = conv.FromFile(raw);

            var canvasId = input.ContentId ?? identifier?.CanvasId
                ?? throw new PluginMisconfigurationException("Canvas ID is missing and could not be inferred from the file.");
            var stepId = input.StepId ?? identifier?.StepId
                ?? throw new PluginMisconfigurationException("Step ID is required for canvas uploads (provide explicitly or via file metadata).");
            var mid = input.MessageVariationId ?? identifier?.MessageVariationId
                ?? throw new PluginMisconfigurationException("Message variation ID is required for canvas uploads (provide explicitly or via file metadata).");

            var getReq = new RestRequest("/canvas/translations")
                .AddQueryParameter("workflow_id", canvasId)
                .AddQueryParameter("step_id", stepId)
                .AddQueryParameter("message_variation_id", mid);
            var getRes = await Client.ExecuteWithErrorHandling<TranslationsDto>(getReq);

            var localeVariant = getRes?.Translations?.FirstOrDefault(x => x.Locale.LocaleKey == input.Locale);
            if (localeVariant == null)
                throw new PluginMisconfigurationException($"The locale '{input.Locale}' is not present on this canvas message.");

            var putReq = new RestRequest("/canvas/translations", Method.Put);
            putReq.AddJsonBody(new
            {
                workflow_id = canvasId,
                message_variation_id = mid,
                locale_id = localeVariant.Locale.Uuid,
                step_id = stepId,
                translation_map = translationMap
            });

            await Client.ExecuteWithErrorHandling(putReq);
        }

        public async Task UploadEmailTemplateAsync(UploadContentRequest input)
        {
            if (input.Content is null)
                throw new PluginMisconfigurationException("Content file is required.");
            if (string.IsNullOrWhiteSpace(input.ContentId))
                throw new PluginMisconfigurationException("Email template ID is required for email_template uploads.");

            var (raw, ext) = await ReadContentTextAsync(input.Content);

            string html = raw;
            if (ext.Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                var dto = JsonConvert.DeserializeObject<EmailTemplateDto>(raw)
                          ?? throw new PluginMisconfigurationException("Invalid JSON uploaded for email_template: deserialization failed.");
                html = dto.Body ?? string.Empty;
            }

            var payload = new Dictionary<string, object>
            {
                ["email_template_id"] = input.ContentId,
                ["body"] = html
            };

            var updateRequest = new RestRequest("/templates/email/update", Method.Post);
            updateRequest.AddJsonBody(payload);

            await Client.ExecuteWithErrorHandling(updateRequest);
        }

        public async Task<(string Text, string Extension)> ReadContentTextAsync(FileReference fileRef)
        {
            using var stream = await fileManagementClient.DownloadAsync(fileRef);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var bytes = ms.ToArray();

            var name = fileRef.Name ?? "content";
            var ext = Path.GetExtension(name).ToLowerInvariant();
            var text = Encoding.UTF8.GetString(bytes);

            if (Xliff2Serializer.IsXliff2(text))
            {
                text = Transformation.Parse(text, name).Target().Serialize()
                       ?? throw new PluginMisconfigurationException("XLIFF did not contain files");
                ext = ".html"; 
            }

            return (text, ext);
        }

        public async Task<DownloadContentResponse> DownloadCampaignAsync(DownloadContentRequest input)
        {
            if (string.IsNullOrWhiteSpace(input.ContentId))
                throw new PluginMisconfigurationException("Content ID is required for campaign downloads.");
            if (string.IsNullOrWhiteSpace(input.Locale))
                throw new PluginMisconfigurationException("Locale is required.");

            var mid = await ResolveMessageVariationIdAsync(input.ContentId, input.MessageVariationId);

            var request = new RestRequest("/campaigns/translations", Method.Get)
                .AddQueryParameter("campaign_id", input.ContentId)
                .AddQueryParameter("message_variation_id", mid);

            var result = await Client.ExecuteWithErrorHandling<TranslationsDto>(request);
            var localeVariant = result?.Translations?.FirstOrDefault(x => x.Locale.LocaleKey == input.Locale);
            if (localeVariant == null)
                throw new PluginMisconfigurationException(
                    $"The locale '{input.Locale}' is not present on this campaign message.");

            var identifier = new CampaignMessageIdentifier
            {
                CampaignId = input.ContentId,
                MessageVariationId = mid
            };

            var jsonConv = ConverterFactory<CampaignMessageIdentifier>.CreateConverter(".json", fileManagementClient);
            var htmlConv = ConverterFactory<CampaignMessageIdentifier>.CreateConverter(".html", fileManagementClient);

            var jsonFile = await jsonConv.ToFile(identifier, localeVariant.TranslationMap);
            var htmlFile = await htmlConv.ToFile(identifier, localeVariant.TranslationMap);

            return new DownloadContentResponse { JsonFile = jsonFile, Content = htmlFile };
        }

        public async Task<DownloadContentResponse> DownloadCanvasAsync(DownloadContentRequest input)
        {
            if (string.IsNullOrWhiteSpace(input.ContentId))
                throw new PluginMisconfigurationException("Canvas ID is required for canvas downloads.");
            if (string.IsNullOrWhiteSpace(input.StepId))
                throw new PluginMisconfigurationException("Step ID is required for canvas downloads.");
            if (string.IsNullOrWhiteSpace(input.MessageVariationId))
                throw new PluginMisconfigurationException("Message variation ID is required for canvas downloads.");
            if (string.IsNullOrWhiteSpace(input.Locale))
                throw new PluginMisconfigurationException("Locale is required.");


            var request = new RestRequest("/canvas/translations")
            .AddQueryParameter("step_id", input.StepId)
            .AddQueryParameter("workflow_id", input.ContentId)
            .AddQueryParameter("message_variation_id", input.MessageVariationId);


            var result = await Client.ExecuteWithErrorHandling<TranslationsDto>(request);
            var localeVariant = result?.Translations?.FirstOrDefault(x => x.Locale.LocaleKey == input.Locale);
            if (localeVariant == null)
                throw new PluginMisconfigurationException($"The locale '{input.Locale}' is not present on this canvas message.");


            var identifier = new CanvasMessageIdentifier
            {
                CanvasId = input.ContentId!,
                MessageVariationId = input.MessageVariationId!,
                StepId = input.StepId!
            };


            var jsonConv = ConverterFactory<CanvasMessageIdentifier>.CreateConverter(".json", fileManagementClient);
            var htmlConv = ConverterFactory<CanvasMessageIdentifier>.CreateConverter(".html", fileManagementClient);


            var jsonFile = await jsonConv.ToFile(identifier, localeVariant.TranslationMap);
            var htmlFile = await htmlConv.ToFile(identifier, localeVariant.TranslationMap);


            return new DownloadContentResponse { JsonFile = jsonFile, Content = htmlFile };
        }

        public async Task<DownloadContentResponse> DownloadEmailTemplateAsync(DownloadContentRequest input)
        {
            if (string.IsNullOrWhiteSpace(input.ContentId))
                throw new PluginMisconfigurationException(
                    "Email template ID is required for email_template downloads.");

            var request = new RestRequest("/templates/email/info");
            request.AddQueryParameter("email_template_id", input.ContentId);

            var dto = await Client.ExecuteWithErrorHandling<EmailTemplateDto>(request);
            if (dto == null)
                throw new PluginApplicationException(
                    $"Email template '{input.ContentId}' not found. Please check your input and try again.");

            var html = dto.Body ?? string.Empty;
            var htmlBytes = Encoding.UTF8.GetBytes(html);
            var htmlFile = await fileManagementClient.UploadAsync(
                new MemoryStream(htmlBytes),
                "text/html",
                $"{input.ContentId}.html");

            var json = JsonConvert.SerializeObject(dto);
            var jsonFile = await fileManagementClient.UploadAsync(
                new MemoryStream(Encoding.UTF8.GetBytes(json)),
                "application/json",
                $"{input.ContentId}.json");

            return new DownloadContentResponse
            {
                JsonFile = jsonFile,
                Content = htmlFile
            };
        }

        public async Task<string> ResolveMessageVariationIdAsync(string campaignId, string? messageVariationId)
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

        public async Task<IEnumerable<ContentItem>> FetchCampaigns(DateTime? editedAfter)
        {
            var request = new RestRequest("/campaigns/list");
            if (editedAfter.HasValue)
            {
                request.AddQueryParameter("last_edit.time[gt]", editedAfter.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
            }

            var res = await Client.ExecuteWithErrorHandling<CampaignListDto>(request);
            if (res?.Campaigns == null)
                return Enumerable.Empty<ContentItem>();

            return res.Campaigns.Select(c => new ContentItem
            {
                ContentId = c.Id,
                ContentType = "campaign",
                Name = c.Name,
                Tags = c.Tags ?? Enumerable.Empty<string>(),
                LastEdited = c.LastEdited
            });
        }

        public async Task<IEnumerable<ContentItem>> FetchCanvases(DateTime? editedAfter)
        {
            var request = new RestRequest("/canvas/list");
            if (editedAfter.HasValue)
            {
                request.AddQueryParameter("last_edit.time[gt]", editedAfter.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
            }

            var res = await Client.ExecuteWithErrorHandling<CanvasListDto>(request);
            if (res?.Canvases == null)
                return Enumerable.Empty<ContentItem>();

            return res.Canvases.Select(c => new ContentItem
            {
                ContentId = c.Id,
                ContentType = "canvas",
                Name = c.Name,
                Tags = c.Tags ?? Enumerable.Empty<string>(),
                LastEdited = c.LastEdited
            });
        }

        public async Task<IEnumerable<ContentItem>> FetchEmailTemplates(DateTime? modifiedAfter, DateTime? modifiedBefore, int? limit, int? offset)
        {
            var request = new RestRequest("/templates/email/list");

            if (modifiedAfter.HasValue)
                request.AddQueryParameter("modified_after", modifiedAfter.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));

            if (modifiedBefore.HasValue)
                request.AddQueryParameter("modified_before", modifiedBefore.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));

            if (limit.HasValue)
            {
                if (limit.Value < 1 || limit.Value > 1000)
                    throw new PluginMisconfigurationException("Limit must be between 1 and 1000.");
                request.AddQueryParameter("limit", limit.Value.ToString());
            }

            if (offset.HasValue)
            {
                if (offset.Value < 0)
                    throw new PluginMisconfigurationException("Offset cannot be negative.");
                request.AddQueryParameter("offset", offset.Value.ToString());
            }

            var res = await Client.ExecuteWithErrorHandling<EmailTemplateListDto>(request);
            if (res?.Templates == null)
                return Enumerable.Empty<ContentItem>();

            var templates = res.Templates.ToList();

            return templates.Select(t => new ContentItem
            {
                ContentId = t.Id,
                ContentType = "email_template",
                Name = t.Name,
                Tags = t.Tags ?? Enumerable.Empty<string>(),
                CreatedAt = t.CreatedAt,
                LastEdited = t.UpdatedAt
            });
        }
    }
}
