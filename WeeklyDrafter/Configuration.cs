using Tomlyn;
using weekly_drafter.Utils;

namespace weekly_drafter;

public class Configuration
{
  private string? _renderedWeeklyUpdatePath;

  // The path where to create weekly updates
  public string? WeeklyUpdatePath { get; init; }

  // The path where to create weekly updates with the {{ date }} placeholder rendered
  public string? RenderedWeeklyUpdatePath
  {
    get
    {
      // Init value
      if (_renderedWeeklyUpdatePath == null && WeeklyUpdatePath != null)
        _renderedWeeklyUpdatePath = Templates.RenderLiquidFromText(WeeklyUpdatePath, new
        {
          Date = Dates.GetMonday()
        });
      return _renderedWeeklyUpdatePath;
    }
  }

  // Extra writers that should contribute to the weekly update
  public List<string>? AdditionalWriters { get; set; }

  // Teams
  public List<Team>? Teams { get; set; }

  // Parse a configuration from a TOML file
  public static Configuration ParseConfiguration(string path)
  {
    // Try to parse the file and return it
    if (Toml.TryToModel<Configuration>(File.ReadAllText(path), out var config, out var errors)) return config;

    // Spit out errors
    foreach (var error in errors)
    {
      Logger.Error(error.Message, new Logger.AnnotationProperties()
      {
        File = path,
        StartLine = error.Span.Start.Line.ToString(),
        EndLine = error.Span.End.Line.ToString(),
        StartColumn = error.Span.Start.Column.ToString(),
        EndColumn = error.Span.End.Column.ToString()
      });
    }

    throw Logger.Error($"Failed to parse configuration at {path}", throws: true)!;
  }

  public class Team
  {
    // Name of the team
    public string? Name { get; set; }

    // Writers responsible for the team's weekly update
    public List<string>? Writers { get; set; }
  }
}