public class Create
{
  public Create(Configuration configuration, GitHub github)
  {
    Configuration = configuration;
    GitHub = github;
  }

  private Configuration Configuration { get; }

  private GitHub GitHub { get; }

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