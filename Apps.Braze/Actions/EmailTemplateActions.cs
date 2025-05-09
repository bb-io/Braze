using Apps.Braze.Api;
using Apps.Braze.Dtos;
using Apps.Braze.Models.EmailTemplates;
using Apps.Braze.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;
using System.Net.Mime;
using System.Text;

namespace Apps.Braze.Actions;
public class EmailTemplateActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
{

    [Action("Search email templates", Description = "Retrieves a list of email templates filtered by modification dates, limit, and offset.")]
    public async Task<EmailTemplateListDto> SearchEmailTemplates([ActionParameter] SearchEmailTemplatesRequest input)
    {
        var request = new RestRequest("/templates/email/list");

        if (input.ModifiedAfter.HasValue)
            request.AddQueryParameter("modified_after", input.ModifiedAfter.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));

        if (input.ModifiedBefore.HasValue)
            request.AddQueryParameter("modified_before", input.ModifiedBefore.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));

        if (input.Limit.HasValue)
        {
            if (input.Limit.Value < 1 || input.Limit.Value > 1000)
                throw new PluginMisconfigurationException("Limit must be between 1 and 1000.");
            request.AddQueryParameter("limit", input.Limit.Value.ToString());
        }

        if (input.Offset.HasValue)
        {
            if (input.Offset.Value < 0)
                throw new PluginMisconfigurationException("Offset cannot be negative.");
            request.AddQueryParameter("offset", input.Offset.Value.ToString());
        }

        var response = await Client.ExecuteWithErrorHandling<EmailTemplateListDto>(request);

        if (response?.Templates == null)
            return new EmailTemplateListDto { Count = 0, Templates = new List<ListTemplate>() };

        var templates = response.Templates
            .OrderBy(t => t.Name)
            .ToList();

        return new EmailTemplateListDto
        {
            Count = response.Count,
            Templates = templates
        };
    }


    [Action("Create email template", Description = "Creates a new email template")]
    public async Task<CreateEmailTemplateResponse> CreateEmailTemplate(
       [ActionParameter] CreateEmailTemplateRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.TemplateName))
            throw new PluginMisconfigurationException("TemplateName is required.");
        if (string.IsNullOrWhiteSpace(input.Subject))
            throw new PluginMisconfigurationException("Subject is required.");

        if (input.HtmlBody == null && string.IsNullOrWhiteSpace(input.StringBody))
            throw new PluginMisconfigurationException("Either StringBody or HtmlBody file must be provided.");

        string htmlContent;
        if (input.HtmlBody != null)
        {
            using var sourceStream = await fileManagementClient.DownloadAsync(input.HtmlBody);
            using var ms = new MemoryStream();
            await sourceStream.CopyToAsync(ms);

            var htmlBytes = ms.ToArray();
            htmlContent = Encoding.UTF8.GetString(htmlBytes);
        }
        else
        {
            htmlContent = input.StringBody;
        }

        var request = new RestRequest("/templates/email/create", Method.Post);
        request.AddJsonBody(new
        {
            template_name = input.TemplateName,
            subject = input.Subject,
            body = htmlContent,
            plaintext_body = input.PlaintextBody,
            preheader = input.Preheader,
            tags = input.Tags
        });

        var dto = await Client.ExecuteWithErrorHandling<CreateEmailTemplateResponseDto>(request);
        if (dto == null || string.IsNullOrWhiteSpace(dto.EmailTemplateId))
            throw new PluginApplicationException("Failed to create email template.");

        return new CreateEmailTemplateResponse
        {
            EmailTemplateId = dto.EmailTemplateId
        };
    }


    [Action("Get email template", Description = "Retrieves information about an email template, including the body as HTML.")]
    public async Task<EmailTemplateResponse> GetEmailTemplate([ActionParameter] EmailTemplateRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.EmailTemplateId))
            throw new PluginMisconfigurationException("Email template ID is required. Please check your input and try again");

        var request = new RestRequest("/templates/email/info");
        request.AddQueryParameter("email_template_id", input.EmailTemplateId);
        var dto = await Client.ExecuteWithErrorHandling<EmailTemplateDto>(request);
        if (dto == null)
            throw new PluginApplicationException("Email template not found. Please check your input and try again");

        var htmlBytes = Encoding.UTF8.GetBytes(dto.Body);
        var fileReference = await fileManagementClient.UploadAsync(new MemoryStream(htmlBytes),"text/html",$"{input.EmailTemplateId}.html");

        return new EmailTemplateResponse
        {
            EmailTemplateId = dto.EmailTemplateId,
            TemplateName = dto.TemplateName,
            Subject = dto.Subject,
            Description = dto.Description,
            Preheader = dto.Preheader,
            PlaintextBody = dto.PlaintextBody,
            ShouldInlineCss = dto.ShouldInlineCss,
            Tags = dto.Tags ?? new List<string>(),
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            Message = dto.Message,
            Body = fileReference
        };
    }

    [Action("Add translation tags to email template", Description = "Goes through the email template and adds the required {% translation } tags.")]
    public async Task AddTranslationTagsToEmailTemplate([ActionParameter] EmailTemplateRequest input)
    {
        var request = new RestRequest("/templates/email/info");
        request.AddQueryParameter("email_template_id", input.EmailTemplateId);

        var result = await Client.ExecuteWithErrorHandling<EmailTemplateDto>(request);

        var newBody = TranslationTagService.AddTranslationTags(result.Body);

        var updateRequest = new RestRequest("/templates/email/update", Method.Post);
        updateRequest.AddJsonBody(new
        {
            email_template_id = input.EmailTemplateId,
            body = newBody,
        });

        await Client.ExecuteWithErrorHandling(updateRequest);
    }

    [Action("Add tags to email template", Description = "Adds tags to an email template.")]
    public async Task AddTagsToEmailTemplate([ActionParameter] EmailTemplateRequest emailTemplate,
        [ActionParameter] EmailTemplateTagsRequest tagsRequest)
    {
        if (string.IsNullOrWhiteSpace(emailTemplate.EmailTemplateId))
            throw new PluginMisconfigurationException("Email template ID is required. Please check your input and try again");
        if (tagsRequest.Tags == null || !tagsRequest.Tags.Any())
            throw new PluginMisconfigurationException("At least one tag is required. Please check your input and try again");

        var updateRequest = new RestRequest("/templates/email/update", Method.Post);
        updateRequest.AddJsonBody(new
        {
            email_template_id = emailTemplate.EmailTemplateId,
            tags = tagsRequest.Tags
        });

        await Client.ExecuteWithErrorHandling(updateRequest);
    }

    [Action("Remove tags from email template", Description = "Removes specified tags from an email template.")]
    public async Task RemoveTagsFromEmailTemplate([ActionParameter] EmailTemplateRequest emailTemplate,
        [ActionParameter] EmailTemplateTagsRequest tagsRequest)
    {
        if (string.IsNullOrWhiteSpace(emailTemplate.EmailTemplateId))
            throw new PluginMisconfigurationException("Email template ID is required. Please check your input and try again");
        if (tagsRequest.Tags == null || !tagsRequest.Tags.Any())
            throw new PluginMisconfigurationException("At least one tag to remove is required. Please check your input and try again");

        var request = new RestRequest("/templates/email/info");
        request.AddQueryParameter("email_template_id", emailTemplate.EmailTemplateId);

        var result = await Client.ExecuteWithErrorHandling<EmailTemplateDto>(request);
        if (result == null)
            throw new PluginApplicationException("Email template not found.");

        var currentTags = result.Tags ?? new List<string>();
        var tagsToRemove = tagsRequest.Tags.Select(t => t.Trim().ToLower()).ToHashSet();
        var updatedTags = currentTags.Where(t => !tagsToRemove.Contains(t.Trim().ToLower())).ToList();

        var updateRequest = new RestRequest("/templates/email/update", Method.Post);
        updateRequest.AddJsonBody(new
        {
            email_template_id = emailTemplate.EmailTemplateId,
            tags = updatedTags
        });

        await Client.ExecuteWithErrorHandling(updateRequest);
    }


    [Action("Update email template", Description = "Updates an existing email template (you can update metadata and/or body via string or HTML file)")]
    public async Task UpdateEmailTemplate(
        [ActionParameter] UpdateEmailTemplateRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.EmailTemplateId))
            throw new PluginMisconfigurationException("EmailTemplateId is required.");

        if (input.TemplateName == null
            && input.Subject == null
            && input.StringBody == null
            && input.HtmlBody == null
            && input.PlaintextBody == null
            && input.Preheader == null
            && input.Tags == null)
        {
            throw new PluginMisconfigurationException(
                "You must supply at least one field to update (e.g. TemplateName, Subject, StringBody/HtmlBody, etc.).");
        }

        string? htmlContent = null;
        if (input.HtmlBody != null)
        {
            using var sourceStream = await fileManagementClient.DownloadAsync(input.HtmlBody);
            using var ms = new MemoryStream();
            await sourceStream.CopyToAsync(ms);
            htmlContent = Encoding.UTF8.GetString(ms.ToArray());
        }
        else if (!string.IsNullOrWhiteSpace(input.StringBody))
        {
            htmlContent = input.StringBody;
        }

        var payload = new Dictionary<string, object>
        {
            ["email_template_id"] = input.EmailTemplateId
        };
        if (!string.IsNullOrWhiteSpace(input.TemplateName))
            payload["template_name"] = input.TemplateName;
        if (!string.IsNullOrWhiteSpace(input.Subject))
            payload["subject"] = input.Subject;
        if (htmlContent != null)
            payload["body"] = htmlContent;
        if (!string.IsNullOrWhiteSpace(input.PlaintextBody))
            payload["plaintext_body"] = input.PlaintextBody;
        if (!string.IsNullOrWhiteSpace(input.Preheader))
            payload["preheader"] = input.Preheader;
        if (input.Tags != null)
            payload["tags"] = input.Tags;

        var updateRequest = new RestRequest("/templates/email/update", Method.Post);
        updateRequest.AddJsonBody(payload);
        await Client.ExecuteWithErrorHandling(updateRequest);
    }
}
