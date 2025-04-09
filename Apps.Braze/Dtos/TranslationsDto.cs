using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Dtos;
public class TranslationsDto
{
    [JsonProperty("translations")]
    public List<Translation> Translations { get; set; }
}

public class Translation
{
    [JsonProperty("translation_map")]
    public Dictionary<string, string> TranslationMap { get; set; }

    [JsonProperty("locale")]
    public Locale Locale { get; set; }
}

public class Locale
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; }

    [JsonProperty("language")]
    public string Language { get; set; }

    [JsonProperty("ca_primary_name")]
    public string CaPrimaryName { get; set; }

    [JsonProperty("ca_primary_value")]
    public string CaPrimaryValue { get; set; }

    [JsonProperty("ca_secondary_name")]
    public string CaSecondaryName { get; set; }

    [JsonProperty("ca_secondary_value")]
    public string CaSecondaryValue { get; set; }

    [JsonProperty("locale_key")]
    public string LocaleKey { get; set; }
}