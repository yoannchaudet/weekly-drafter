using System.Collections.ObjectModel;
using System.Text;

public static class Logger
{
  // Write a debug message
  public static void Debug(string? message)
  {
    IssueCommand(new CommandEnvelope("debug", null, message));
  }

  // Write an error message (potentially along with an annotation)
  public static void Error(string? message, AnnotationProperties? annotationProps = null, bool throws = false)
  {
    IssueCommand(new CommandEnvelope("error", annotationProps.ToDictionary(), message));
    if (throws)
      throw new Exception(message);
  }

  // Write a warning message (potentially along with an annotation)
  public static void Warning(string? message, AnnotationProperties? annotationProps = null)
  {
    IssueCommand(new CommandEnvelope("warning", annotationProps.ToDictionary(), message));
  }

  // Write a notice message (potentially along with an annotation)
  public static void Notice(string? message, AnnotationProperties? annotationProps = null)
  {
    IssueCommand(new CommandEnvelope("notice", annotationProps.ToDictionary(), message));
  }

  // Write an info message
  public static void Info(string message)
  {
    Console.WriteLine(message);
  }

  // Write a group[ and return an IDisposable to be used in a using statement (for closing the group)
  public static IDisposable Group(string name)
  {
    IssueCommand(new CommandEnvelope("group", new Dictionary<string, string> { { "name", name } }, ""));
    return new EndGroupDisposable();
  }

  // Issue a command on the standard output
  private static void IssueCommand(CommandEnvelope command)
  {
    Console.WriteLine(command.ToString());
  }

  // Convert an annotation properties object to a dictionary
  private static Dictionary<string, string> ToDictionary(this AnnotationProperties? annotationProps)
  {
    return annotationProps == null
      ? new Dictionary<string, string>()
      : new Dictionary<string, string>
      {
        { "title", annotationProps.Title ?? "" },
        { "file", annotationProps.File ?? "" },
        { "line", annotationProps.StartLine ?? "" },
        { "endLine", annotationProps.EndLine ?? "" },
        { "col", annotationProps.StartColumn ?? "" },
        { "endColumn", annotationProps.EndColumn ?? "" }
      };
  }

  // Disposable responsible for closing a group
  private class EndGroupDisposable : IDisposable
  {
    public void Dispose()
    {
      IssueCommand(new CommandEnvelope("endgroup", null, ""));
    }
  }

  // Annotation properties object
  public class AnnotationProperties
  {
    public string? Title { get; set; }
    public string? File { get; set; }
    public string? StartLine { get; set; }
    public string? EndLine { get; set; }
    public string? StartColumn { get; set; }
    public string? EndColumn { get; set; }
  }

  // Envelope objects for console commands
  public class CommandEnvelope
  {
    public CommandEnvelope(string command, Dictionary<string, string>? parameters, string? message)
    {
      Command = command;
      Parameters = parameters?.AsReadOnly();
      Message = message;
    }

    public string Command { get; }
    public ReadOnlyDictionary<string, string>? Parameters { get; }
    public string? Message { get; }

    // Output the envelope as a string
    public override string ToString()
    {
      var builder = new StringBuilder();
      builder.Append($"::{Command}");
      if (Parameters?.Count > 0)
      {
        builder.Append(' ');
        builder.AppendJoin(",", Parameters.Select(p => $"{EscapeProperty(p.Key)}={EscapeProperty(p.Value)}"));
      }

      builder.Append("::");
      builder.Append(EscapeData(Message));
      return builder.ToString().TrimEnd();
    }

    private static string EscapeData(string? value)
    {
      return value == null
        ? ""
        : value
          .Replace("%", "%25")
          .Replace("\r", "%0D")
          .Replace("\n", "%0A");
    }

    private static string EscapeProperty(string? value)
    {
      return value == null
        ? ""
        : value
          .Replace("%", "%25")
          .Replace("\r", "%0D")
          .Replace("\n", "%0A")
          .Replace(":", "%3A")
          .Replace(",", "%2C");
    }
  }
}