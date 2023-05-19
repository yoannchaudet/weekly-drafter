using Scriban;
using Tomlyn;

namespace weekly_drafter;

public class Configuration
{
  // The path where to create weekly updates
  public string? WeeklyUpdatePath { get; set; }

  // The path where to create weekly updates with the {{ date }} placeholder rendered
  public string? RenderedWeeklyUpdatePath { get; private set; }

  // Extra writers that should contribute to the weekly update
  public List<string>? AdditionalWriters { get; set; }

  // Teams
  public List<Team>? Teams { get; set; }

  // Parse a configuration from a TOML file
  public static Configuration ParseConfiguration(string path)
  {
    if (Toml.TryToModel<Configuration>(File.ReadAllText(path), out var config, out var errors))
    {
      if (config.WeeklyUpdatePath != null)
      {
        var template = Template.ParseLiquid(config.WeeklyUpdatePath);
        config.RenderedWeeklyUpdatePath = template.Render(new
        {
          Date = Dates.GetMonday().ToSortable()
        });
      }

      return config;
    }
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
  }
}