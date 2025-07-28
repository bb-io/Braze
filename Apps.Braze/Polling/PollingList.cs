using Apps.Braze.Dtos;
using Apps.Braze.Models.Campaigns;
using Apps.Braze.Models.Canvas;
using Apps.Braze.Models.General;
using Apps.Braze.Polling.Memory;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Apps.Braze.Polling
{
    [PollingEventList]
    public class PollingList : Invocable
    {
        public PollingList(InvocationContext invocationContext) : base(invocationContext)
        {
        }


        [PollingEvent("On canvas updated", Description = "Triggers when a canvas is updated")]
        public Task<PollingEventResponse<DateMemory, PollingCanvasResponse>> OnCanvasUpdated(
            PollingEventRequest<DateMemory> request)
            => HandleCanvasUpdatedPolling(request);

        [PollingEvent("On campaign updated" , Description = "Triggers when a campaign is updated")]
        public Task<PollingEventResponse<DateMemory, PollingCampaignResponse>> OnCampaignUpdated(PollingEventRequest<DateMemory> request)
        {
            return HandleCampaignUpdatedPolling(request);
        }

        [PollingEvent("On campaign message tag added", Description = "Triggers when a campaign message tag is added")]
        public Task<PollingEventResponse<DateMemory, PollingCampaignResponse>> OnCampaignTagAdded(
            PollingEventRequest<DateMemory> request,
            [PollingEventParameter] PollingCampaignRequest input)
            => HandleCampaignTagPolling(request, input);

        [PollingEvent("On campaign message translation added or updated", Description = "Triggers when a campaign message is updated")]
        public Task<PollingEventResponse<CampaignMessageMemory, PollingCampaignMessageContentResponse>> OnCampaignMessageContentUpdated(
            PollingEventRequest<CampaignMessageMemory> request,
            [PollingEventParameter] PollingCampaignMessageContentRequest input)
            => HandleCampaignMessageContentPolling(request, input);

        [PollingEvent("On email template tag added", Description = "Triggers when a email template tag is added")]
        public Task<PollingEventResponse<DateMemory, PollingEmailTemplateResponse>> OnEmailTemplateTagAdded(
            PollingEventRequest<DateMemory> request,
            [PollingEventParameter] PollingEmailTemplateRequest input)
            => HandleEmailTemplateTagPolling(request, input);

        [PollingEvent("On canvas message tag added", Description = "Triggers when a canvas message tag is added")]
        public Task<PollingEventResponse<DateMemory, CanvasListDto>> OnCanvasTagAdded(
            PollingEventRequest<DateMemory> request,
            [PollingEventParameter] PollingCanvasRequest input)
            => HandleCanvasTagPolling(request, input);



        private async Task<PollingEventResponse<DateMemory, CanvasListDto>> HandleCanvasTagPolling(
           PollingEventRequest<DateMemory> request,
           PollingCanvasRequest input)
        {
            var rest = new RestRequest("/canvas/list", Method.Get);
            if (request.Memory != null)
            {
                rest.AddQueryParameter(
                    "last_edit.time[gt]",
                    request.Memory.LastInteractionDate.ToString("o"));
            }

            var listResponse = await Client.ExecuteWithErrorHandling<CanvasListDto>(rest);
            var canvases = listResponse.Canvases.ToList();

            if (!string.IsNullOrEmpty(input.CanvasId))
            {
                canvases = canvases
                    .Where(c => string.Equals(c.Id, input.CanvasId, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (input.Tags != null && input.Tags.Any())
            {
                var required = new HashSet<string>(input.Tags, StringComparer.OrdinalIgnoreCase);
                canvases = canvases
                    .Where(c => c.Tags != null
                                && required.Any(rt => c.Tags.Any(t =>
                                    string.Equals(t, rt, StringComparison.OrdinalIgnoreCase))))
                    .ToList();
            }

            if (!canvases.Any())
            {
                if (request.Memory == null)
                {
                    return new PollingEventResponse<DateMemory, CanvasListDto>
                    {
                        FlyBird = false,
                        Memory = new DateMemory
                        {
                            LastInteractionDate = DateTime.UtcNow
                        }
                    };
                }

                return new PollingEventResponse<DateMemory, CanvasListDto>
                {
                    FlyBird = false,
                    Memory = request.Memory
                };
            }

            if (request.Memory == null)
            {
                var maxDate = canvases.Max(c => c.LastEdited);
                return new PollingEventResponse<DateMemory, CanvasListDto>
                {
                    FlyBird = false,
                    Memory = new DateMemory
                    {
                        LastInteractionDate = maxDate
                    }
                };
            }

            var updated = canvases
                .Where(c => c.LastEdited > request.Memory.LastInteractionDate)
                .ToList();

            if (!updated.Any())
            {
                return new PollingEventResponse<DateMemory, CanvasListDto>
                {
                    FlyBird = false,
                    Memory = request.Memory
                };
            }

            var newMax = updated.Max(c => c.LastEdited);
            request.Memory.LastInteractionDate = newMax;

            return new PollingEventResponse<DateMemory, CanvasListDto>
            {
                FlyBird = true,
                Memory = request.Memory,
                Result = new CanvasListDto
                {
                    Canvases = updated
                }
            };
        }


        private async Task<PollingEventResponse<DateMemory, PollingCampaignResponse>> HandleCampaignTagPolling(
            PollingEventRequest<DateMemory> request,
            PollingCampaignRequest input)
        {
            var allCampaigns = new List<ListCampaign>();
            int page = 0;
            bool hasMore = true;

            while (hasMore)
            {
                var restRequest = new RestRequest("/campaigns/list", Method.Get);
                restRequest.AddQueryParameter("page", page.ToString());

                if (request.Memory != null)
                {
                    restRequest.AddQueryParameter(
                        "last_edit.time[gt]",
                        request.Memory.LastInteractionDate.ToString("o"));
                }

                var listResponse = await Client.ExecuteWithErrorHandling<CampaignListDto>(restRequest);
                var campaigns = listResponse.Campaigns.ToList();

                if (campaigns.Any())
                {
                    allCampaigns.AddRange(campaigns);
                    page++;
                }
                else
                {
                    hasMore = false;
                }
            }

            var json = JsonConvert.SerializeObject(allCampaigns, Formatting.Indented);
            Console.WriteLine($"Fetched {allCampaigns.Count} campaigns: {json}");

            if (!string.IsNullOrEmpty(input.CampaignId))
            {
                allCampaigns = allCampaigns
                    .Where(c => string.Equals(c.Id, input.CampaignId, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (input.Tags != null && input.Tags.Any())
            {
                var requiredTags = new HashSet<string>(input.Tags, StringComparer.OrdinalIgnoreCase);
                allCampaigns = allCampaigns
                    .Where(c => c.Tags != null
                                && requiredTags.Any(rt =>
                                    c.Tags.Any(t => string.Equals(t, rt, StringComparison.OrdinalIgnoreCase))))
                    .ToList();
            }

            if (!allCampaigns.Any())
            {
                if (request.Memory == null)
                {
                    return new PollingEventResponse<DateMemory, PollingCampaignResponse>
                    {
                        FlyBird = false,
                        Memory = new DateMemory { LastInteractionDate = DateTime.UtcNow }
                    };
                }

                return new PollingEventResponse<DateMemory, PollingCampaignResponse>
                {
                    FlyBird = false,
                    Memory = request.Memory
                };
            }

            if (request.Memory == null)
            {
                var maxDate = allCampaigns.Max(c => c.LastEdited);
                return new PollingEventResponse<DateMemory, PollingCampaignResponse>
                {
                    FlyBird = false,
                    Memory = new DateMemory { LastInteractionDate = maxDate }
                };
            }

            var updated = allCampaigns
                .Where(c => c.LastEdited > request.Memory.LastInteractionDate)
                .ToArray();

            if (!updated.Any())
            {
                return new PollingEventResponse<DateMemory, PollingCampaignResponse>
                {
                    FlyBird = false,
                    Memory = request.Memory
                };
            }

            request.Memory.LastInteractionDate = updated.Max(c => c.LastEdited);
            return new PollingEventResponse<DateMemory, PollingCampaignResponse>
            {
                FlyBird = true,
                Memory = request.Memory,
                Result = new PollingCampaignResponse(updated)
            };
        }


        private async Task<PollingEventResponse<CampaignMessageMemory, PollingCampaignMessageContentResponse>> HandleCampaignMessageContentPolling(
            PollingEventRequest<CampaignMessageMemory> request,
            PollingCampaignMessageContentRequest input)
        {
            var restRequest = new RestRequest("/campaigns/list", Method.Get);
            if (request.Memory != null)
                restRequest.AddQueryParameter("last_edit.time[gt]", request.Memory.LastInteractionDate.ToString("o"));

            var response = await Client.ExecuteWithErrorHandling<CampaignListDto>(restRequest);
            var campaigns = response.Campaigns.ToList();

            if (input.Tags != null && input.Tags.Any())
            {
                var requiredTags = new HashSet<string>(input.Tags, StringComparer.OrdinalIgnoreCase);
                campaigns = campaigns
                    .Where(c => c.Tags?.Intersect(requiredTags, StringComparer.OrdinalIgnoreCase).Any() ?? false)
                    .ToList();
            }

            if (campaigns.Count == 0)
            {
                var memory = request.Memory ?? new CampaignMessageMemory { LastInteractionDate = DateTime.UtcNow };

                return new PollingEventResponse<CampaignMessageMemory, PollingCampaignMessageContentResponse>
                {
                    FlyBird = false,
                    Memory = memory
                };
            }

            // This is a first run: initialize memory and return early
            if (request.Memory == null)
            {
                var maxDate = campaigns.Max(c => c.LastEdited);
                return new PollingEventResponse<CampaignMessageMemory, PollingCampaignMessageContentResponse>
                {
                    FlyBird = false,
                    Memory = new CampaignMessageMemory { LastInteractionDate = maxDate }
                };
            }

            var updatedCampaigns = campaigns
                .Where(c => c.LastEdited > request.Memory.LastInteractionDate)
                .ToList();

            if (updatedCampaigns.Count == 0)
            {
                return new PollingEventResponse<CampaignMessageMemory, PollingCampaignMessageContentResponse>
                {
                    FlyBird = false,
                    Memory = request.Memory
                };
            }

            var updatedMessages = new List<ListCampaignMessageDto>();
            var lastInteractionDate = updatedCampaigns.Max(c => c.LastEdited);
            var currentTime = DateTime.UtcNow;

            // For each updated campaign, fetch translations, convert to json and hash that json
            // Event will fly only with messages that are not already in memory by comparing their hashes
            foreach (var campaign in updatedCampaigns)
            {
                var campaignRequest = new RestRequest("/campaigns/details");
                campaignRequest.AddQueryParameter("campaign_id", campaign.Id);
                var campaignDetails = await Client.ExecuteWithErrorHandling<CampaignDto>(campaignRequest);

                foreach (var message in campaignDetails.MessageVariations)
                {
                    var translationRequest = new RestRequest("/campaigns/translations", Method.Get);
                    translationRequest.AddQueryParameter("campaign_id", campaign.Id);
                    translationRequest.AddQueryParameter("message_variation_id", message.Id);

                    TranslationsDto translationResponse;
                    try
                    {
                        translationResponse = await Client.ExecuteWithErrorHandling<TranslationsDto>(translationRequest);
                    }
                    catch (PluginApplicationException ex)
                    when (ex.Message.Contains("This message does not have multi-language setup")
                        || ex.Message.Contains("Duplicate translation IDs"))
                    {
                        continue;
                    }

                    var localeVariant = translationResponse.Translations.FirstOrDefault(x => x.Locale.LocaleKey == input.Locale);
                    if (localeVariant == null)
                    {
                        continue;
                    }

                    var identifier = new CampaignMessageIdentifier
                    {
                        CampaignId = campaign.Id,
                        MessageVariationId = message.Id
                    };

                    var representation = new JsonFileRepresentation<CampaignMessageIdentifier> { Meta = identifier, TranslationMap = localeVariant.TranslationMap };
                    var messageJson = JsonConvert.SerializeObject(representation, Formatting.Indented);
                    var messageJsonBytes = Encoding.UTF8.GetBytes(messageJson);

                    var messageHash = SHA512.HashData(messageJsonBytes);
                    var messageHex = Convert.ToHexString(messageHash);

                    if (request.Memory.CampaignMessages.TryAdd(messageHex, currentTime))
                    {
                        var messageForList = new MessageListDto
                        {
                            Id = message.Id,
                            Name = message.Name,
                            Channel = message.Channel,
                        };
                        updatedMessages.Add(new ListCampaignMessageDto { Message = messageForList, Campaign = campaign });
                    }  
                }
            }

            // Remove messages older than a year from memory
            foreach (var memorizedMessage in request.Memory.CampaignMessages)
            {
                if (memorizedMessage.Value.AddYears(1) < currentTime)
                {
                    request.Memory.CampaignMessages.Remove(memorizedMessage.Key);
                }
            }

            request.Memory.LastInteractionDate = lastInteractionDate;
            return new PollingEventResponse<CampaignMessageMemory, PollingCampaignMessageContentResponse>
            {
                FlyBird = true,
                Memory = request.Memory,
                Result = new PollingCampaignMessageContentResponse(updatedMessages)
            };
        }


        private async Task<PollingEventResponse<DateMemory, PollingEmailTemplateResponse>> HandleEmailTemplateTagPolling(
            PollingEventRequest<DateMemory> request,
            PollingEmailTemplateRequest input)
        {
            var restRequest = new RestRequest("/templates/email/list");
            if (request.Memory != null)
            {
                restRequest.AddQueryParameter(
                    "modified_after",
                    request.Memory.LastInteractionDate.ToString("o"));
            }

            var listResponse = await Client.ExecuteWithErrorHandling<EmailTemplateListDto>(restRequest);
            var templates = listResponse.Templates.ToList();

            if (!string.IsNullOrEmpty(input.EmailTemplateId))
            {
                templates = templates
                    .Where(c => string.Equals(c.Id, input.EmailTemplateId, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (input.Tags != null && input.Tags.Any())
            {
                var requiredTags = new HashSet<string>(input.Tags, StringComparer.OrdinalIgnoreCase);
                templates = templates
                    .Where(c => c.Tags != null
                                && requiredTags.Any(rt =>
                                    c.Tags.Any(t => string.Equals(t, rt, StringComparison.OrdinalIgnoreCase))))
                    .ToList();
            }

            if (!templates.Any())
            {
                if (request.Memory == null)
                {
                    return new PollingEventResponse<DateMemory, PollingEmailTemplateResponse>
                    {
                        FlyBird = false,
                        Memory = new DateMemory { LastInteractionDate = DateTime.UtcNow }
                    };
                }

                return new PollingEventResponse<DateMemory, PollingEmailTemplateResponse>
                {
                    FlyBird = false,
                    Memory = request.Memory
                };
            }

            if (request.Memory == null)
            {
                var maxDate = templates.Max(c => c.UpdatedAt);
                return new PollingEventResponse<DateMemory, PollingEmailTemplateResponse>
                {
                    FlyBird = false,
                    Memory = new DateMemory { LastInteractionDate = maxDate }
                };
            }

            var updated = templates
                .Where(c => c.UpdatedAt > request.Memory.LastInteractionDate)
                .ToArray();

            if (!updated.Any())
            {
                return new PollingEventResponse<DateMemory, PollingEmailTemplateResponse>
                {
                    FlyBird = false,
                    Memory = request.Memory
                };
            }

            request.Memory.LastInteractionDate = updated.Max(c => c.UpdatedAt);
            return new PollingEventResponse<DateMemory, PollingEmailTemplateResponse>
            {
                FlyBird = true,
                Memory = request.Memory,
                Result = new PollingEmailTemplateResponse(updated)
            };
        }

        private async Task<PollingEventResponse<DateMemory, PollingCanvasResponse>> HandleCanvasUpdatedPolling(
           PollingEventRequest<DateMemory> request)
        {
            var restRequest = new RestRequest("/canvas/list", Method.Get);

            if (request.Memory != null)
            {
                restRequest.AddQueryParameter(
                    "last_edit.time[gt]",
                    request.Memory.LastInteractionDate.ToString("o")
                );
            }

            var response = await Client.ExecuteWithErrorHandling<CanvasListDto>(restRequest);
            var allCanvases = response.Canvases.ToArray();

            if (request.Memory == null)
            {
                var maxDate = allCanvases.Any()
                    ? allCanvases.Max(c => c.LastEdited)
                    : DateTime.UtcNow;

                return new PollingEventResponse<DateMemory, PollingCanvasResponse>
                {
                    FlyBird = false,
                    Memory = new DateMemory { LastInteractionDate = maxDate }
                };
            }

            var newCanvases = allCanvases
                .Where(c => c.LastEdited > request.Memory.LastInteractionDate)
                .ToArray();

            if (newCanvases.Any())
            {
                var newMax = newCanvases.Max(c => c.LastEdited);
                request.Memory.LastInteractionDate = newMax;

                return new PollingEventResponse<DateMemory, PollingCanvasResponse>
                {
                    FlyBird = true,
                    Memory = request.Memory,
                    Result = new PollingCanvasResponse(newCanvases)
                };
            }

            return new PollingEventResponse<DateMemory, PollingCanvasResponse>
            {
                FlyBird = false,
                Memory = request.Memory
            };
        }

        private async Task<PollingEventResponse<DateMemory, PollingCampaignResponse>> HandleCampaignUpdatedPolling(
            PollingEventRequest<DateMemory> request)
        {
            var restRequest = new RestRequest("/campaigns/list", Method.Get);
            if (request.Memory != null)
                restRequest.AddQueryParameter("last_edit.time[gt]", request.Memory.LastInteractionDate.ToString("o"));

            var response = await Client.ExecuteWithErrorHandling<CampaignListDto>(restRequest);
            var all = response.Campaigns.ToArray();

            if (request.Memory == null)
            {
                var max = all.Any() ? all.Max(c => c.LastEdited) : DateTime.UtcNow;
                return new PollingEventResponse<DateMemory, PollingCampaignResponse>
                {
                    FlyBird = false,
                    Memory = new DateMemory { LastInteractionDate = max }
                };
            }

            var news = all.Where(c => c.LastEdited > request.Memory.LastInteractionDate).ToArray();
            if (news.Any())
            {
                var max = news.Max(c => c.LastEdited);
                request.Memory.LastInteractionDate = max;
                return new PollingEventResponse<DateMemory, PollingCampaignResponse>
                {
                    FlyBird = true,
                    Memory = request.Memory,
                    Result = new PollingCampaignResponse(news)
                };
            }

            return new PollingEventResponse<DateMemory, PollingCampaignResponse>
            {
                FlyBird = false,
                Memory = request.Memory
            };
        }
    }

}
