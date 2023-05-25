using weekly_drafter.Services;

namespace weekly_drafter.Commands;

public class Create
{
  public Create(ActionContext actionContext, Configuration configuration, GitHub github)
  {
    ActionContext = actionContext;
    Configuration = configuration;
    GitHub = github;
  }

  private Configuration Configuration { get; }

  private GitHub GitHub { get; }

  private ActionContext ActionContext { get; }

  public async Task Run()
  {
    // Get the current date
    var sortableMonday = Dates.GetMonday().ToSortable();
    Logger.Info($"Attempting to create a weekly update for {sortableMonday}");

    // Check if we have a PR already
    var pr = await GitHub.GetCurrentWeeklyUpdatePR(sortableMonday);
    if (pr != null)
    {
      Logger.Warning($"A weekly update PR already exist for {sortableMonday}! See {pr.Url}");
      return;
    }

    //
    // Open the PR!
    //
    pr = await GitHub.CreatePullRequest();
    Logger.Info($"PR created at {pr.Url}!");
  }
}