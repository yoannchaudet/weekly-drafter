namespace weekly_drafter;

public static class Constants
{
  // The configuration path
  public const string ConfigurationPath = ".github/weekly-drafter/config.toml";

  // Weekly template path
  public const string WeeklyTemplatePath = ".github/weekly-drafter/weekly.liquid";

  // Weekly PR template path
  public const string WeeklyPRTemplatePath = ".github/weekly-drafter/weekly.pr.liquid";

  // Label that PRs are created with
  public const string WeeklyUpdateLabel = "weekly-update";

  // Label color that PRs are created with
  public const string WeeklyUpdateLabelColor = "A511C0";

  // PR body markers
  public const string WeeklyUpdateMarker = "weekly-update";
  public const string WeeklyUpdateMarkerDate = "date";

  // API product header
  public const string GitHubAPIProductHeader = "weekly-update";
}