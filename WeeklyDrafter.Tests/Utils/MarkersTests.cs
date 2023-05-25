namespace WeeklyDrafter.Tests.Utils;

public class MarkersTests
{
  [Fact]
  public void ToTextSimple()
  {
    var marker = new Markers.Marker("test");
    Assert.Equal("<!-- test -->", Markers.ToText(marker));
  }

  [Fact]
  public void ToTextArguments()
  {
    var marker = new Markers.Marker("test").AddArgument("foo", "bar");
    Assert.Equal("<!-- test foo=bar -->", Markers.ToText(marker));
    marker = new Markers.Marker("test").AddArgument("foo", "bar").AddArgument("unsafe&", "?");
    Assert.Equal("<!-- test foo=bar&unsafe&=%3f -->", Markers.ToText(marker));
  }

  [Fact]
  public void FromTextSimple()
  {
    var text = "<!-- hello -->";
    var markers = Markers.FromText(text);
    Assert.Single(markers);
    Assert.Equal("hello", markers.First().Name);
    Assert.Equal(0, markers.First().Start);
    Assert.Equal(0, markers.First().Line);
    Assert.Equal(text.Length, markers.First().Length);
    Assert.Equal(text.Length, markers.First().End);
  }

  [Fact]
  public void FromTextComplex()
  {
    var text = @"
# Hello

<!-- marker1 date=2020-05-16 -->

- item 1
- item 2 <!-- marker2 -->

<!-- /marker3 -->
@";
    var markers = Markers.FromText(text);
    Assert.Equal(3, markers.Count());
    var marker1 = markers.First(m => m.Name == "marker1");
    var marker2 = markers.First(m => m.Name == "marker2");
    var marker3 = markers.First(m => m.Name == "/marker3");

    // marker1
    Assert.Equal("2020-05-16", marker1.Arguments.First(a => a.Key == "date").Value);
    Assert.Equal(13, marker1.Start);
    Assert.Equal(3, marker1.Line);
    Assert.Equal("<!-- marker1 date=2020-05-16 -->".Length, marker1.Length);

    // marker2
    Assert.Empty(marker2.Arguments);
    Assert.Equal(68, marker2.Start);
    Assert.Equal(6, marker2.Line);

    // marker3
    Assert.Empty(marker3.Arguments);
    Assert.Equal(88, marker3.Start);
    Assert.Equal(8, marker3.Line);
  }
}