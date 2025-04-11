using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Services;
public static class TranslationTagService
{
    private static readonly string[] BlockTags = new[] {
        "address", "article", "aside", "blockquote", "canvas", "dd", "div", "dl",
        "dt", "fieldset", "figcaption", "figure", "footer", "form", "h1", "h2", "h3",
        "h4", "h5", "h6", "header", "hr", "li", "main", "nav", "noscript", "ol", "p",
        "pre", "section", "table", "tfoot", "ul", "video", "tr", "td"
    };

    public static string AddTranslationTags(string originalHtml)
    {
        var segmentCounter = 1;

        var doc = new HtmlDocument();
        doc.LoadHtml(originalHtml);

        // Find candidate nodes that are:
        // - Block-level
        // - Don't contain nested block-level tags
        // - Don't already contain a {% translation tag
        var nodes = doc.DocumentNode.Descendants()
            .Where(n =>
                n.NodeType == HtmlNodeType.Element &&
                BlockTags.Contains(n.Name.ToLower()) &&
                !n.Descendants().Any(d =>
                    d != n && d.NodeType == HtmlNodeType.Element && BlockTags.Contains(d.Name.ToLower())
                ) &&
                !n.InnerHtml.Contains("{% translation")
            )
            .ToList();

        foreach (var node in nodes)
        {
            node.InnerHtml = $"{{% translation id_{segmentCounter} %}}{node.InnerHtml}{{% endtranslation %}}";
            segmentCounter++;
        }

        return doc.DocumentNode.OuterHtml;
    }
}
