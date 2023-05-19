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
}