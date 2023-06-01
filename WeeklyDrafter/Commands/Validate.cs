using weekly_drafter.Services;

namespace weekly_drafter.Commands;

public class Validate
{
  public Validate(ActionsContext actionsContext, Configuration configuration, GitHub github)
  {
    ActionsContext = actionsContext;
    Configuration = configuration;
    GitHub = github;
  }

  private Configuration Configuration { get; }

  private GitHub GitHub { get; }

  private ActionsContext ActionsContext { get; }

  public async Task Run()
  {
   // WIP do validation here!
   await Task.Delay(TimeSpan.FromSeconds(1));

   // Logging
   Logger.Info("Validation completed!");
  }
}