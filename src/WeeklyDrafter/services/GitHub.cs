using Autofac.Features.AttributeFilters;
using Octokit.GraphQL;
using Octokit.GraphQL.Model;

public class GitHub : IGitHub
{
  private ActionContext Context { get; }
  private Connection Connection { get; }

  public GitHub(ActionContext context)
  {
    this.Context = context;

    // Create a connection
    this.Connection = new Connection(new ProductHeaderValue("weekly-drafter"), context.GitHubToken);
  }

  public async Task<IEnumerable<IGitHub.PullRequest>> GetLastWeeklyUpdatePRs(int first = 10)
  {
    // PR filters
    var orderBy = new IssueOrder
    {
      Direction = OrderDirection.Desc,
      Field = IssueOrderField.CreatedAt
    };
    var labels = new string[] { Constants.WEEKLY_UPDATE_LABEL };
    var states = new PullRequestState[] { PullRequestState.Open };

    // Select last PRs still opened with the weekly-update label
    var query = new Query()
      .Repository(this.Context.GitHubRepositoryName, this.Context.GitHubRepositoryOwnerName)
      .PullRequests(orderBy: orderBy, labels: labels, states: states, first: first)
      .Nodes
      .Select(pr => new IGitHub.PullRequest
      {
        Body = pr.Body,
        Url = pr.Url,
        Number = pr.Number,
      });
    return await Connection.Run(query);
  }
}