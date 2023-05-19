using System.Collections.Specialized;
using System.Text;
using Scriban;
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
    var pr = await GitHub.GetCurrentWeeklyUpdatePr(sortableMonday);
    if (pr != null)
    {
      Logger.Warning($"A weekly update PR already exist for {sortableMonday}! See {pr.Url}");
      return;
    }

    //
    // Prepare a new PR!
    //

    // Parse the template file and render it
    var templatePath = Path.Join(ActionContext.GitHubWorkspace, Constants.WeeklyTemplatePath);
    if (!File.Exists(templatePath))
    {
      Logger.Error("Unable to find a template file", new Logger.AnnotationProperties { File = templatePath });
      return;
    }

    var template = Template.ParseLiquid(await File.ReadAllTextAsync(templatePath));

    // Render the template
    var templateInputs = new
    {
      config = Configuration,
      mondayEnglish = Dates.GetMonday().ToEnglish(),
      mondaySortable = Dates.GetMonday().ToSortable()
    };
    var renderedTemplate = new StringBuilder((await template.RenderAsync(templateInputs)).Trim());

    // Append a marker with the date to the template
    var weeklyUpdateMarker = new Markers.Marker(Constants.WeeklyUpdateMarker,
      new NameValueCollection { { Constants.WeeklyUpdateMarkerDate, sortableMonday } });
    renderedTemplate.Append(Environment.NewLine);
    renderedTemplate.AppendLine(Markers.ToText(weeklyUpdateMarker));

    //
    // Open the PR!
    //

    await GitHub.CreatePullRequest();
  }
}