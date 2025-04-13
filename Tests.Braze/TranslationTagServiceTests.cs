using Apps.Braze.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Braze;

[TestClass]
public class TranslationTagServiceTests
{
    [TestMethod]
    public void TestWrapSimpleBlock()
    {
        var html = "<p>Hello <b>world</b>!</p>";
        var expected = "<p>{% translation id_1 %}Hello <b>world</b>!{% endtranslation %}</p>";

        var result = TranslationTagService.AddTranslationTags(html);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void TestNestedBlockNotWrapped()
    {
        var html = "<div><p>Inside</p></div>";
        var expected = "<div><p>{% translation id_1 %}Inside{% endtranslation %}</p></div>";

        var result = TranslationTagService.AddTranslationTags(html);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void TestMultipleBlocks()
    {
        var html = "<p>First</p><div><p>Second</p></div><p>Third <i>inline</i></p>";
        var expected = "<p>{% translation id_1 %}First{% endtranslation %}</p>" +
                       "<div><p>{% translation id_2 %}Second{% endtranslation %}</p></div>" +
                       "<p>{% translation id_3 %}Third <i>inline</i>{% endtranslation %}</p>";

        var result = TranslationTagService.AddTranslationTags(html);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void TestExistingTagsNotEdited()
    {
        var html = "<p>{% translation id_1 %}First{% endtranslation %}</p>" +
                       "<div><p>{% translation id_2 %}Second{% endtranslation %}</p></div>" +
                       "<p>{% translation id_3 %}Third <i>inline</i>{% endtranslation %}</p>";
        var expected = "<p>{% translation id_1 %}First{% endtranslation %}</p>" +
                       "<div><p>{% translation id_2 %}Second{% endtranslation %}</p></div>" +
                       "<p>{% translation id_3 %}Third <i>inline</i>{% endtranslation %}</p>";

        var result = TranslationTagService.AddTranslationTags(html);

        Assert.AreEqual(expected, result);
    }
}
