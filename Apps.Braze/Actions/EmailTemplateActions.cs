using Apps.Braze.Api;
using Apps.Braze.Dtos;
using Apps.Braze.Models.Campaigns;
using Apps.Braze.Services;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Apps.Braze.Models.EmailTemplates;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Braze.Actions;
public class EmailTemplateActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
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

    [Action("Add tags to email template", Description = "Adds Braze template labels to an email template.")]
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
}
