using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Models.General;
public class JsonFileRepresentation<T>
{
    [JsonProperty("_meta")]
    public T Meta { get; set; }

    [JsonProperty("translation_map")]
    public Dictionary<string, string> TranslationMap { get; set; }
}
