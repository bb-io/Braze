using System.Text;
using Apps.Braze.Models.Content;
using Apps.Braze.Models.ContentBlocks;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff2;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Braze.Actions;

[ActionList("Content blocks")]
public class ContentBlockActions(
    InvocationContext invocationContext,
    IFileManagementClient fileManagementClient) : Invocable(invocationContext)
{
    [Action("Search content blocks", Description = "Retrieves all content blocks filtered by modification dates.")]
    public async Task<ContentBlockListResponse> SearchContentBlocks([ActionParameter] SearchContentBlocksRequest input)
    {
        ValidateSearchInput(input);

        const int pageSize = 1000;
        var offset = 0;
        var contentBlocks = new List<ContentBlockListItem>();

        while (true)
        {
            var request = new RestRequest("/content_blocks/list")
                .AddQueryParameter("limit", pageSize.ToString());
            AddSearchParameters(request, input);
            if (offset > 0)
                request.AddQueryParameter("offset", offset.ToString());

            var response = await Client.ExecuteWithErrorHandling<ContentBlockListResponse>(request);
            var page = (response.ContentBlocks ?? []).ToList();
            contentBlocks.AddRange(page);

            if (page.Count < pageSize)
                break;

            offset += pageSize;
        }

        return new ContentBlockListResponse
        {
            Count = contentBlocks.Count,
            ContentBlocks = contentBlocks.OrderBy(x => x.Name).ToList()
        };
    }

    [Action("Get content block", Description = "Retrieves information and content for a content block.")]
    public async Task<ContentBlockInfo> GetContentBlock([ActionParameter] ContentBlockRequest input)
    {
        ValidateContentBlockId(input.ContentBlockId);

        var request = new RestRequest("/content_blocks/info")
            .AddQueryParameter("content_block_id", input.ContentBlockId);
        if (input.IncludeInclusionData.HasValue)
            request.AddQueryParameter("include_inclusion_data", input.IncludeInclusionData.Value.ToString().ToLowerInvariant());

        return await Client.ExecuteWithErrorHandling<ContentBlockInfo>(request);
    }

    [Action("Download content block", Description = "Downloads a content block as a content file and JSON snapshot.")]
    public async Task<DownloadContentResponse> DownloadContentBlock([ActionParameter] ContentBlockRequest input)
    {
        var contentBlock = await GetContentBlock(input);
        var isHtml = string.Equals(contentBlock.ContentType, "html", StringComparison.OrdinalIgnoreCase);
        var extension = isHtml ? ".html" : ".txt";
        var mimeType = isHtml ? "text/html" : "text/plain";

        var contentBytes = Encoding.UTF8.GetBytes(contentBlock.Content ?? string.Empty);
        var contentFile = await fileManagementClient.UploadAsync(
            new MemoryStream(contentBytes), mimeType, $"{contentBlock.ContentBlockId}{extension}");

        var json = JsonConvert.SerializeObject(contentBlock, Formatting.Indented);
        var jsonFile = await fileManagementClient.UploadAsync(
            new MemoryStream(Encoding.UTF8.GetBytes(json)), "application/json", $"{contentBlock.ContentBlockId}.json");

        return new DownloadContentResponse
        {
            Content = contentFile,
            JsonFile = jsonFile
        };
    }

    [Action("Upload content block", Description = "Updates an existing content block from an HTML, text, JSON, or XLIFF file.")]
    public async Task UploadContentBlock([ActionParameter] UploadContentBlockRequest input)
    {
        ValidateContentBlockId(input.ContentBlockId);
        if (input.Content is null)
            throw new PluginMisconfigurationException("Content file is required.");

        var content = await ReadContentAsync(input.Content);
        if (Encoding.UTF8.GetByteCount(content) >= 51200)
            throw new PluginMisconfigurationException("Content must be smaller than 50 KB.");

        var request = new RestRequest("/content_blocks/update", Method.Post);
        request.AddJsonBody(new
        {
            content_block_id = input.ContentBlockId,
            content
        });

        await Client.ExecuteWithErrorHandling(request);
    }

    private async Task<string> ReadContentAsync(Blackbird.Applications.Sdk.Common.Files.FileReference file)
    {
        using var stream = await fileManagementClient.DownloadAsync(file);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);

        var text = Encoding.UTF8.GetString(memoryStream.ToArray());
        var name = file.Name ?? "content";
        var extension = Path.GetExtension(name);

        if (Xliff2Serializer.IsXliff2(text))
        {
            return Transformation.Parse(text, name).Target().Serialize()
                   ?? throw new PluginMisconfigurationException("XLIFF did not contain files.");
        }

        if (!extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
            return text;

        ContentBlockInfo? snapshot;
        try
        {
            snapshot = JsonConvert.DeserializeObject<ContentBlockInfo>(text);
        }
        catch (JsonException exception)
        {
            throw new PluginMisconfigurationException($"Invalid content block JSON: {exception.Message}");
        }

        if (string.IsNullOrEmpty(snapshot?.Content))
            throw new PluginMisconfigurationException("Content block JSON must contain a non-empty 'content' field.");

        return snapshot.Content;
    }

    private static void ValidateContentBlockId(string? contentBlockId)
    {
        if (string.IsNullOrWhiteSpace(contentBlockId))
            throw new PluginMisconfigurationException("Content block ID is required.");
    }

    private static void ValidateSearchInput(SearchContentBlocksRequest input)
    {
        if (input.ModifiedAfter.HasValue
            && input.ModifiedBefore.HasValue
            && input.ModifiedAfter.Value > input.ModifiedBefore.Value)
            throw new PluginMisconfigurationException("Modified after must be earlier than or equal to modified before.");
    }

    private static void AddSearchParameters(RestRequest request, SearchContentBlocksRequest input)
    {
        if (input.ModifiedAfter.HasValue)
            request.AddQueryParameter("modified_after", input.ModifiedAfter.Value.ToString("o"));
        if (input.ModifiedBefore.HasValue)
            request.AddQueryParameter("modified_before", input.ModifiedBefore.Value.ToString("o"));
    }
}
