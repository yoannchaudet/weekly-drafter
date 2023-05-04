using System.Collections.ObjectModel;
using System.Collections.Immutable;
using System.Text;

public static class Logger
{
  // WIP



  // Envelope objects for console commands
  public class CommandEnvelope
  {
    public string Command { get; }
    public ReadOnlyDictionary<string, string> Parameters { get; }
    public string Message { get; }

    public CommandEnvelope(string command, Dictionary<string, string>? parameters, string message)
    {
      Command = command;
      // TODO: in .net 8, use ReadOnlyDictionary.Empty
      Parameters = parameters == null ? new Dictionary<string, string>().AsReadOnly() : parameters.AsReadOnly();
      Message = message;
    }

    // Output the envelope as a string
    public String ToString()
    {
      var builder = new StringBuilder();
      builder.Append($"::{Command}");
      if (Parameters.Count > 0)
      {
        builder.Append(" ");
        builder.AppendJoin(",", Parameters.Select(p => $"{EscapeProperty(p.Key)}={EscapeProperty(p.Value)}"));
      }
      builder.Append(" ");
      builder.Append(EscapeData(Message));
      return builder.ToString().TrimEnd();
    }

    private static string EscapeData(string value)
    {
      return value == null ? "" : value
        .Replace("%", "%25")
        .Replace("\r", "%0D")
        .Replace("\n", "%0A");
    }

    private static string EscapeProperty(string value)
    {
      return value == null ? "" : value
        .Replace("%", "%25")
        .Replace("\r", "%0D")
        .Replace("\n", "%0A")
        .Replace(":", "%3A")
        .Replace(",", "%2C");
    }
  }
}