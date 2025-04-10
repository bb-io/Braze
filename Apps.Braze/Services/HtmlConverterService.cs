using Apps.Braze.Constants;
using Apps.Braze.Models.General;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Services;
public class HtmlConverterService<T>(IFileManagementClient fileManagementClient) : IConverterService<T> where T : IIdentifier
{
    public async Task<FileReference> ToFile(T identifier, Dictionary<string, string> translationMap)
    {
        var (doc, bodyNode) = PrepareEmptyHtmlDocument(identifier);
        foreach (var (key, value) in translationMap)
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.InnerHtml = value;
            node.SetAttributeValue(ConvertConstants.TranslationKeyAttribute, key);
            bodyNode.AppendChild(node);
        }

        return await fileManagementClient.UploadAsync(new MemoryStream(
            Encoding.UTF8.GetBytes(doc.DocumentNode.OuterHtml)),
            MediaTypeNames.Text.Html, $"{identifier.GetId()}.html"
            );
    }

    public (T, Dictionary<string, string>) FromFile(string fileContent)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(fileContent);

        var identifier = Activator.CreateInstance<T>();

        foreach (var property in typeof(T).GetProperties())
        {
            var value = doc.DocumentNode.SelectSingleNode($"//meta[@name='blackbird-{property.Name}']")?.GetAttributeValue("content", null) ?? "";
            property.SetValue(identifier, value);
        }

        var translationMap = doc.DocumentNode.Descendants()
            .Where(x => x.Attributes[ConvertConstants.TranslationKeyAttribute] is not null)
            .ToDictionary(x => x.Attributes[ConvertConstants.TranslationKeyAttribute].Value, x => x.InnerHtml);

        return (identifier, translationMap);
    }

    private (HtmlDocument document, HtmlNode bodyNode) PrepareEmptyHtmlDocument(T identifier)
    {
        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement(HtmlConstants.Html);
        htmlDoc.DocumentNode.AppendChild(htmlNode);

        var headNode = htmlDoc.CreateElement(HtmlConstants.Head);
        htmlNode.AppendChild(headNode);

        foreach( var property in typeof(T).GetProperties() )
        {
            var key = property.Name;
            var value = property.GetValue(identifier)?.ToString();
            var metaNode = htmlDoc.CreateElement("meta");
            metaNode.SetAttributeValue("name", $"blackbird-{key}");
            metaNode.SetAttributeValue("content", value);
            headNode.AppendChild(metaNode);
        }

        var bodyNode = htmlDoc.CreateElement(HtmlConstants.Body);
        htmlNode.AppendChild(bodyNode);

        return (htmlDoc, bodyNode);
    }
}


