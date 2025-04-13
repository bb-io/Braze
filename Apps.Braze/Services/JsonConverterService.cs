using Apps.Braze.Models.Campaigns;
using Apps.Braze.Models.General;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Services;
public class JsonConverterService<T>(IFileManagementClient fileManagementClient) : IConverterService<T> where T : IIdentifier
{
    public async Task<FileReference> ToFile(T identifier, Dictionary<string, string> translationMap)
    {
        var representation = new JsonFileRepresentation<T> { Meta = identifier, TranslationMap = translationMap };
        var outputModelJson = JsonConvert.SerializeObject(representation, Formatting.Indented);
        return await fileManagementClient.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes(outputModelJson)), MediaTypeNames.Application.Json, $"{identifier.GetId()}.json");
    }

    public (T, Dictionary<string, string>) FromFile(string fileContent)
    {
        var json = JsonConvert.DeserializeObject<JsonFileRepresentation<T>>(fileContent);
        return (json.Meta,  json.TranslationMap);
    }
}
