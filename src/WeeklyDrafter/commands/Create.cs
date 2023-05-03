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
    var sortableMonday = Dates.GetMonday().ToSortable();
    var pr = await GitHub.GetCurrentWeeklyUpdatePR(sortableMonday);
    if (pr == null)
      Console.WriteLine("no PR");
    else
      Console.WriteLine($"PR matching {pr.Number}");
  }
}