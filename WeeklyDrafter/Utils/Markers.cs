using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

// An utility for creating and parsing markers in arbitrary text files (typically Markdown here)
public static class Markers
{
  // Marker pattern
  private static readonly Regex MarkerPattern =
    new(@"<!--\s*(?<name>[^\s]+)(\s+(?<args>[^\s]+))?\s*-->", RegexOptions.Compiled);

  // Convert a marker into its text representation
  public static string ToText(Marker marker)
  {
    var builder = new StringBuilder();
    builder.Append($"<!-- {marker.Name}");
    if (marker.Arguments.Keys.Count > 0)
    {
      builder.Append(" ");
      builder.Append(string.Join("&",
        marker.Arguments.AllKeys.Select(a => $"{a}={HttpUtility.UrlEncode(marker.Arguments[a])}")));
    }

    builder.Append(" -->");
    return builder.ToString();
  }

  // Extract markers from a given text
  public static IEnumerable<MarkerSpan> FromText(string text)
  {
    return MarkerPattern.Matches(text).Select(m =>
    {
      var name = m.Groups["name"].Value;
      var args = HttpUtility.ParseQueryString(m.Groups["args"].Value);
      return new MarkerSpan(name, args, m.Index, m.Length);
    });
  }

  public class Marker
  {
    public Marker(string name) : this(name, new NameValueCollection())
    {
    }

    public Marker(string name, NameValueCollection arguments)
    {
      Name = name;
      Arguments = arguments;
    }

    // Name of the marker
    public string Name { get; }

    // Arguments of the marker
    public NameValueCollection Arguments { get; }
  }

  public class MarkerSpan : Marker
  {
    public MarkerSpan(string name, NameValueCollection arguments, int start, int length) : base(name, arguments)
    {
      Start = start;
      Length = length;
    }

    public int Start { get; }
    public int Length { get; }
    public int End => Start + Length;
  }
}