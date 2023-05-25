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
}