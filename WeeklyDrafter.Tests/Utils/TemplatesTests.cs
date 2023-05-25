using weekly_drafter.Utils;

namespace WeeklyDrafter.Tests.Utils;

public class TemplatesTests
{
  [Fact]
  public void TextTemplateTrimsAndReplacesVariables()
  {
    var template = Templates.RenderLiquidFromText(" Hello {{ name }} ", new { Name = "Yoann" });
    Assert.Equal("Hello Yoann", template);
  }

  [Fact]
  public void DateUtilsSortable()
  {
    var now = new DateTime();
    var template = Templates.RenderLiquidFromText("{{ now | dates.sortable }}", new { Now = now });
    Assert.Equal(now.ToSortable(), template);
  }

  [Fact]
  public void DateUtilsEnglish()
  {
    var now = new DateTime();
    var template = Templates.RenderLiquidFromText("{{ now | dates.english }}", new { Now = now });
    Assert.Equal(now.ToEnglish(), template);
  }

  [Fact]
  public void MarkerUtils()
  {
    var marker = new Markers.Marker("hello").AddArgument("foo", "bar");
    var template =
      Templates.RenderLiquidFromText(
        "{{ markers.new 'hello' | markers.add_argument 'foo' 'bar' | markers.to_string }}");
    Assert.Equal(Markers.ToText(marker), template);
  }


  [Fact]
  public void MarkerJoining()
  {
    var people = new[] { "1", "2" }.ToList();
    var separator = '-';
    var marker = new Markers.Marker("hello").AddArgument("foo", "bar")
      .AddArgument("people", string.Join(separator, people));
    var template =
      Templates.RenderLiquidFromText(
        "{{ markers.new 'hello' | markers.add_argument 'foo' 'bar' | markers.join_argument 'people', people, separator | markers.to_string }}",
        new
        {
          People = people,
          Separator = separator
        });
    Assert.Equal(Markers.ToText(marker), template);
  }
}