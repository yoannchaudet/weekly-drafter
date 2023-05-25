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
    if (marker.Arguments.Any())
    {
      builder.Append(" ");
      builder.Append(string.Join("&",
        marker.Arguments.Select(a => $"{a.Key}={HttpUtility.UrlEncode(a.Value)}")));
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
      var arguments = new Dictionary<string, string>();
      foreach (string? o in args) arguments.Add(o!, args[o!]!);
      var line = Regex.Matches(text.Substring(0, m.Index), @"\n").Cast<Match>().Count();
      // Note: GitHub uses 1-index lines so increment
      return new MarkerSpan(name, arguments, m.Index, m.Length, line + 1);
    });
  }

  public class Marker
  {
    private readonly Dictionary<string, string> _arguments;

    internal Marker(string name, Dictionary<string, string> arguments)
    {
      Name = name;
      _arguments = arguments;
    }

    public Marker(string name) : this(name, new Dictionary<string, string>())
    {
    }

    // Name of the marker
    public string Name { get; }

    // Arguments
    public IEnumerable<KeyValuePair<string, string>> Arguments => _arguments;

    // Add an argument
    public Marker AddArgument(string key, string value)
    {
      _arguments[key] = value;
      return this;
    }
  }

  public class MarkerSpan : Marker
  {
    public MarkerSpan(string name, Dictionary<string, string> arguments, int start, int length, int line) : base(name, arguments)
    {
      Start = start;
      Length = length;
      Line = line;
    }

    public int Start { get; }
    public int Length { get; }
    public int Line { get;  }
    public int End => Start + Length;
  }
}