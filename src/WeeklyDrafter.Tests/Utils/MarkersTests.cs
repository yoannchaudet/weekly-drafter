namespace WeeklyDrafter.Tests;

using System.Collections.Specialized;

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
    var marker = new Markers.Marker("test", new NameValueCollection { { "foo", "bar" } });
    Assert.Equal("<!-- test foo=bar -->", Markers.ToText(marker));

    marker = new Markers.Marker("test", new NameValueCollection { { "foo", "bar" }, { "unsafe&", "?" } });
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
    var marker1 = markers.Where(m => m.Name == "marker1").First();
    var marker2 = markers.Where(m => m.Name == "marker2").First();
    var marker3 = markers.Where(m => m.Name == "/marker3").First();

    // marker1
    Assert.Equal("2020-05-16", marker1.Arguments["date"]);
    Assert.Equal(13, marker1.Start);
    Assert.Equal("<!-- marker1 date=2020-05-16 -->".Length, marker1.Length);

    // marker2
    Assert.Empty(marker2.Arguments);
    Assert.Equal(68, marker2.Start);

    // marker3
    Assert.Empty(marker3.Arguments);
    Assert.Equal(88, marker3.Start);
  }
}