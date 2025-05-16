using Apps.Braze.Models.Campaigns;
using Apps.Braze.Models.General;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Services;
public static class ConverterFactory<T> where T : IIdentifier
{
    public static IConverterService<T> CreateConverter(string fileExtension, IFileManagementClient fileManagementClient)
    {
        if (fileExtension.ToLower() == ".json")
            return new JsonConverterService<T>(fileManagementClient);
        else if (fileExtension.ToLower() == ".html")
            return new HtmlConverterService<T>(fileManagementClient);
        else throw new PluginMisconfigurationException($"The file format {fileExtension} is not supported. Use a transformed file that was exported from the 'Download' action.");
    }
}
