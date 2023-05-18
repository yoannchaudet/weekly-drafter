using Octokit.GraphQL;
using Octokit.GraphQL.Model;

namespace weekly_drafter.Services;

public class GitHub
{
  public GitHub(ActionContext context)
  {
    Context = context;

    // Create a connection
    Connection = new Connection(new ProductHeaderValue("weekly-drafter"), context.GitHubToken);
  }

  private ActionContext Context { get; }
  private Connection Connection { get; }

  // Return the last few opened weekly update PRs
  public async Task<IEnumerable<PullRequest>> GetLastWeeklyUpdatePRs(int first = 10)
  {
    // PR filters
    var orderBy = new IssueOrder
    {
      Direction = OrderDirection.Desc,
      Field = IssueOrderField.CreatedAt
    };
    var labels = new[] { Constants.WeeklyUpdateLabel };
    var states = new[] { PullRequestState.Open };

    // Select last PRs still opened with the weekly-update label
    var query = new Query()
      .Repository(Context.GitHubRepositoryName, Context.GitHubRepositoryOwnerName)
      .PullRequests(orderBy: orderBy, labels: labels, states: states, first: first)
      .Nodes
      .Select(pr => new PullRequest
      {
        Body = pr.Body,
        Url = pr.Url,
        Number = pr.Number
      });
    return await Connection.Run(query);
  }

  // Return the current weekly update PR if any
  public async Task<PullRequest?> GetCurrentWeeklyUpdatePr(string sortableMonday)
  {
    return (await GetLastWeeklyUpdatePRs()).FirstOrDefault(pr =>
      Markers.FromText(pr.Body!).Where(m =>
          m.Name == Constants.WeeklyUpdateMarker &&
          m.Arguments[Constants.WeeklyUpdateMarkerDate] == sortableMonday)
        .Count() > 0
    );
  }

  public class PullRequest
  {
    public string? Body { get; set; }
    public string? Url { get; set; }
    public int Number { get; set; }
  }
}