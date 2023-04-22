public class Create : ICommand
{
  private Configuration Configuration { get; }

  private IGitHub GitHub { get; }

  public Create(Configuration configuration, IGitHub github)
  {
    this.Configuration = configuration;
    this.GitHub = github;
  }

  public async Task Run()
  {

    var pr = await this.GitHub.GetLastWeeklyUpdatePRs();
    Console.WriteLine("test");

  }
}