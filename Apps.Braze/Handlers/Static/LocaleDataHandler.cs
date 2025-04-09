using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Braze.Handlers.Static;
public class LocaleDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>()
        {
            new DataSourceItem("en", "English"),
            new DataSourceItem("af", "Afrikaas"),
            new DataSourceItem("agq", "Aghem"),
            new DataSourceItem("ak", "Akan"),
            new DataSourceItem("sq", "Albanian"),
            new DataSourceItem("am", "Amharic"),
            new DataSourceItem("ar", "Arabic"),
            new DataSourceItem("hy", "Armenian"),
            new DataSourceItem("as", "Assamese"),
            new DataSourceItem("ay", "Aymara"),
            new DataSourceItem("az", "Azerbaijani"),
            new DataSourceItem("ksf", "Bafia"),
            new DataSourceItem("bas", "Basa"),
            new DataSourceItem("eu", "Basque"),
            new DataSourceItem("be", "Belarusian"),
            new DataSourceItem("bem", "Bemba"),
            new DataSourceItem("bn", "Bengali"),
            new DataSourceItem("bez", "Bena"),
            new DataSourceItem("bs", "Bosnian"),
            new DataSourceItem("br", "Breton"),
            new DataSourceItem("bg", "Bulgarian"),
            new DataSourceItem("my", "Burmese"),
            new DataSourceItem("km", "Cambodian"),
            new DataSourceItem("ca", "Catalan"),
            new DataSourceItem("tzm", "Central Atlas Tamazight"),
            new DataSourceItem("chr", "Cherokee"),
            new DataSourceItem("cgg", "Chiga"),
            new DataSourceItem("zh", "Chinese"),
            new DataSourceItem("swc", "Congo Swahili"),
            new DataSourceItem("kw", "Cornish"),
            new DataSourceItem("hr", "Croatian"),
            new DataSourceItem("cs", "Czech"),
            new DataSourceItem("da", "Danish"),
            new DataSourceItem("dav", "Dawida"),
            new DataSourceItem("dua", "Douala"),
            new DataSourceItem("nl", "Dutch"),
            new DataSourceItem("dz", "Dzongkha"),
            new DataSourceItem("guz", "Ekugusii"),
            new DataSourceItem("et", "Estonian"),
            new DataSourceItem("eo", "Esperanto"),
            new DataSourceItem("ewo", "Ewondo"),
            new DataSourceItem("ee", "Ewe"),
            new DataSourceItem("fo", "Faroese"),
            new DataSourceItem("fa", "Farsi"),
            new DataSourceItem("fil", "Filipino"),
            new DataSourceItem("fi", "Finnish"),
            new DataSourceItem("fr", "French"),
            new DataSourceItem("gl", "Galician"),
            new DataSourceItem("lg", "Ganda"),
            new DataSourceItem("ka", "Georgian"),
            new DataSourceItem("de", "German"),
            new DataSourceItem("gsw", "German Swiss"),
            new DataSourceItem("el", "Greek"),
            new DataSourceItem("kl", "Greenlandic"),
            new DataSourceItem("gn", "Guarani"),
            new DataSourceItem("gu", "Gujarati"),
            new DataSourceItem("ha", "Hausa"),
            new DataSourceItem("haw", "Hawaiian"),
            new DataSourceItem("he", "Hebrew"),
            new DataSourceItem("hi", "Hindi"),
            new DataSourceItem("hu", "Hungarian"),
            new DataSourceItem("is", "Icelandic"),
            new DataSourceItem("ig", "Igbo"),
            new DataSourceItem("id", "Indonesian"),
            new DataSourceItem("iu", "Inuktitut"),
            new DataSourceItem("ga", "Irish"),
            new DataSourceItem("it", "Italian"),
            new DataSourceItem("jv", "Javanese"),
            new DataSourceItem("ja", "Japanese"),
            new DataSourceItem("dyo", "Jola_fonyi"),
            new DataSourceItem("kab", "Kabyle"),
            new DataSourceItem("kln", "Kalenjin"),
            new DataSourceItem("kam", "Kamba"),
            new DataSourceItem("kn", "Kannada"),
            new DataSourceItem("ks", "Kashmiri"),
            new DataSourceItem("kk", "Kazakh"),
            new DataSourceItem("ebu", "Kiembu"),
            new DataSourceItem("ki", "Kikuyu"),
            new DataSourceItem("rw", "Kinyarwanda"),
            new DataSourceItem("ky", "Kirghiz"),
            new DataSourceItem("ko", "Korean"),
            new DataSourceItem("ku", "Kurdish"),
            new DataSourceItem("lo", "Lao"),
            new DataSourceItem("la", "Latin"),
            new DataSourceItem("lv", "Latvian"),
            new DataSourceItem("ln", "Lingala"),
            new DataSourceItem("lt", "Lithuanian"),
            new DataSourceItem("lu", "Luba Katanga"),
            new DataSourceItem("lb", "Luxembourgish"),
            new DataSourceItem("luo", "Luo"),
            new DataSourceItem("luy", "Luyia"),
            new DataSourceItem("jmc", "Machame"),
            new DataSourceItem("mk", "Macedonian"),
            new DataSourceItem("mg", "Malagasy"),
            new DataSourceItem("ms", "MALAY"),
            new DataSourceItem("ml", "Malay"),
            new DataSourceItem("mt", "Maltese"),
            new DataSourceItem("gv", "Manx"),
            new DataSourceItem("mr", "Marathi"),
            new DataSourceItem("mas", "Masai"),
            new DataSourceItem("mer", "Meru"),
            new DataSourceItem("mo", "Moldavian"),
            new DataSourceItem("mn", "Mongolian"),
            new DataSourceItem("mfe", "Morisyen"),
            new DataSourceItem("mua", "Mundang"),
            new DataSourceItem("naq", "Nam"),
            new DataSourceItem("ne", "Nepali"),
            new DataSourceItem("nd", "North Ndebele"),
            new DataSourceItem("nb", "Norwegian"),
            new DataSourceItem("nus", "Nuer"),
            new DataSourceItem("nyn", "Nyankole"),
            new DataSourceItem("nn", "Nynorsk"),
            new DataSourceItem("om", "Oromo"),
            new DataSourceItem("ps", "Pashto"),
            new DataSourceItem("ff", "Peul"),
            new DataSourceItem("pl", "Polish"),
            new DataSourceItem("pt", "Portuguese"),
            new DataSourceItem("pa", "Punjabi"),
            new DataSourceItem("qu", "Quechua"),
            new DataSourceItem("rm", "Raeto Romance"),
            new DataSourceItem("ro", "Romanian"),
            new DataSourceItem("rof", "Rombo"),
            new DataSourceItem("ru", "Russian"),
            new DataSourceItem("rwk", "Rwa"),
            new DataSourceItem("saq", "Samburu"),
            new DataSourceItem("se", "Sami"),
            new DataSourceItem("sbp", "Sangu"),
            new DataSourceItem("sa", "Sanskrit"),
            new DataSourceItem("gd", "Scottish"),
            new DataSourceItem("sr", "Serbian"),
            new DataSourceItem("seh", "Sena"),
            new DataSourceItem("ksb", "Shambala"),
            new DataSourceItem("sn", "Shona"),
            new DataSourceItem("ii", "Sichuan Yi"),
            new DataSourceItem("sd", "Sindhi"),
            new DataSourceItem("si", "Sinhalese"),
            new DataSourceItem("sk", "Slovak"),
            new DataSourceItem("sl", "Slovenian"),
            new DataSourceItem("so", "Somali"),
            new DataSourceItem("es", "Spanish"),
            new DataSourceItem("sw", "Swahili"),
            new DataSourceItem("sv", "Swedish"),
            new DataSourceItem("shi", "Tachelhit"),
            new DataSourceItem("tl", "Tagalog"),
            new DataSourceItem("tg", "Tajiki"),
            new DataSourceItem("ta", "Tamil"),
            new DataSourceItem("twq", "Tasawaq"),
            new DataSourceItem("tt", "Tatar"),
            new DataSourceItem("te", "Telugu"),
            new DataSourceItem("teo", "Teso"),
            new DataSourceItem("th", "Thai"),
            new DataSourceItem("bo", "Tibetan"),
            new DataSourceItem("ti", "Tigrinya"),
            new DataSourceItem("to", "Tongan"),
            new DataSourceItem("tr", "Turkish"),
            new DataSourceItem("tk", "Turkmen"),
            new DataSourceItem("ug", "Uighur"),
            new DataSourceItem("uk", "Ukrainian"),
            new DataSourceItem("ur", "Urdu"),
            new DataSourceItem("uz", "Uzbek"),
            new DataSourceItem("vai", "Vai"),
            new DataSourceItem("vi", "Vietnamese"),
            new DataSourceItem("vun", "Vunjo"),
            new DataSourceItem("cy", "Welsh"),
            new DataSourceItem("xh", "Xhosa"),
            new DataSourceItem("yav", "Yangben"),
            new DataSourceItem("yi", "Yiddish"),
            new DataSourceItem("yo", "Yoruba"),
            new DataSourceItem("dje", "Zarma"),
            new DataSourceItem("zu", "Zulu"),
        };
    }
}
