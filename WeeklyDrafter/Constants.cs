namespace weekly_drafter;

public static class Constants
{
  // The configuration path
  public const string ConfigurationPath = ".github/weekly-drafter/config.toml";

  // Weekly template path
  public const string WeeklyTemplatePath = ".github/weekly-drafter/weekly.liquid";

  // Weekly PR template path
  public const string WeeklyPullRequestTemplatePath = ".github/weekly-drafter/weekly.pr.liquid";

  // Weekly placeholder template path
  public const string WeeklyPlaceholderTemplatePath = ".github/weekly-drafter/weekly.placeholder.liquid";

  // Label that PRs are created with
  public const string WeeklyUpdateLabel = "weekly-update";

  // Label color that PRs are created with
  public const string WeeklyUpdateLabelColor = "A511C0";

  // PR body markers
  public const string WeeklyUpdateMarker = "weekly-update";
  public const string WeeklyUpdateMarkerDate = "date";

  // API product header
  public const string GitHubApiProductHeader = "weekly-update";
}