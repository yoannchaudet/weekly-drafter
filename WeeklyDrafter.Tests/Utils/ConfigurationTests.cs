using weekly_drafter;

namespace WeeklyDrafter.Tests.Utils;

public class ConfigurationTests
{
  [Fact]
  public void RenderedWeeklyUpdatePath()
  {
    Assert.Null(new Configuration().RenderedWeeklyUpdatePath);
    var conf = new Configuration { WeeklyUpdatePath = "some/path/{{date}}.md" };
    Assert.Equal($"some/path/{Dates.GetMonday().ToSortable()}.md", conf.RenderedWeeklyUpdatePath);
  }
}