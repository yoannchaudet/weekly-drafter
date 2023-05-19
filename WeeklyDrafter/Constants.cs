namespace weekly_drafter;

public static class Constants
{
  // The configuration path
  public const string ConfigurationPath = ".github/weekly-drafter/config.toml";

  // Weekly template path
  public const string WeeklyTemplatePath = ".github/weekly-drafter/weekly.liquid";

  // Label that PRs are created with
  public const string WeeklyUpdateLabel = "weekly-update";

  // PR body markers
  public const string WeeklyUpdateMarker = "weekly-update";
  public const string WeeklyUpdateMarkerDate = "date";
}