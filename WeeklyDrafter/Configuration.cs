using System.Collections.Specialized;
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
          Date = Dates.GetMonday().ToSortable()
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
    if (Toml.TryToModel<Configuration>(File.ReadAllText(path), out var config, out var errors)) return config;

    // WIP: parse the errors and create proper errors
    // https://docs.github.com/en/actions/using-workflows/workflow-commands-for-github-actions#setting-an-error-message
    throw new Exception($"Failed to parse configuration file at {path}:\n{string.Join("\n", errors)}");
  }

  public class Team
  {
    // Name of the team
    public string? Name { get; set; }

    // Writers responsible for the team's weekly update
    public List<string>? Writers { get; set; }

    // Return a placeholder
    public string Marker
    {
      get
      {
        var args = new NameValueCollection();
          if (Writers != null)
            args.Add("writers", string.Join(',', Writers!));
          var marker = new Markers.Marker("placeholder", args);
          return Markers.ToText(marker);
       }
    }
  }
}