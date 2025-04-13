using Apps.Braze.Constants;
using Apps.Braze.Models.General;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Services;
public interface IConverterService<T> where T : IIdentifier
{
    public Task<FileReference> ToFile(T identifier, Dictionary<string, string> translationMap);

    public (T, Dictionary<string, string>) FromFile(string fileContent);
}
