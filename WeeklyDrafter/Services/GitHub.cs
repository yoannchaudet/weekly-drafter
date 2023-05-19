using System.Text;
using Octokit.GraphQL;
using Octokit.GraphQL.Model;

namespace weekly_drafter.Services;

public class GitHub
{
  public GitHub(ActionContext context, Configuration configuration)
  {
    Context = context;
    Configuration = configuration;

    // Create a connection
    Connection = new Connection(new ProductHeaderValue("weekly-drafter"), context.GitHubToken);
  }

  private ActionContext Context { get; }
  private Connection Connection { get; }

  private Configuration Configuration { get; }

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
      Markers.FromText(pr.Body!)
        .Count(m => m.Name == Constants.WeeklyUpdateMarker &&
                    m.Arguments[Constants.WeeklyUpdateMarkerDate] == sortableMonday) > 0
    );
  }

  // Return basic information about the current repository (e.g. default branch's name)
  public async Task<Repository> GetRepository()
  {
    var query = new Query()
      .Repository(Context.GitHubRepositoryName, Context.GitHubRepositoryOwnerName)
      .DefaultBranchRef
      .Select(reference => new Repository
      {
        RepositoryId = reference.Repository.Id,
        DefaultBranchRef = reference.Name,
        DefaultBranchOid = reference.Target.Oid
      });
    return await Connection.Run(query);
  }

  public async Task<PullRequest> CreatePullRequest()
  {
    var uniqueBranchName = $"{Dates.GetMonday().ToSortable()}__{Guid.NewGuid().ToString()}";
    var repository = await GetRepository();

    // Create a branch
    var baseRefName = $"refs/heads/{uniqueBranchName}";
    var createRef = new Mutation()
      .CreateRef(new CreateRefInput
      {
        RepositoryId = repository.RepositoryId!.Value,
        Name = baseRefName,
        Oid = repository.DefaultBranchOid
      }).Select(reference => reference.Ref.Id);
    var branchReferenceId = await Connection.Run(createRef);

    var plainTextBytes = Encoding.UTF8.GetBytes("Hello my friend");
    var content = Convert.ToBase64String(plainTextBytes);

    // Add a commit on that branch
    var createCommitOnBranch = new Mutation()
      .CreateCommitOnBranch(new CreateCommitOnBranchInput
      {
        Branch = new CommittableBranch
        {
          Id = branchReferenceId
        },
        Message = new CommitMessage
        {
          Headline = "Pouet",
          Body = "Pouet body"
        },
        ExpectedHeadOid = repository.DefaultBranchOid,
        FileChanges = new FileChanges
        {
          // Add the weekly update file
          Additions = new[]
          {
            new FileAddition
            {
              Path = Configuration.RenderedWeeklyUpdatePath,
              Contents = content
            }
          },
          Deletions = new FileDeletion[] { }
        }
      }).Select(c => c.Commit.Oid);
    var commitReferenceId = await Connection.Run(createCommitOnBranch);

    var x = new Mutation().CreatePullRequest(new CreatePullRequestInput
    {
      RepositoryId = repository.RepositoryId!.Value,
      BaseRefName = repository.DefaultBranchRef,
      HeadRefName = baseRefName,
      Title = "Hello title",
      Body = "hello body",
      MaintainerCanModify = true,
      Draft = false
    }).Select(pr =>
      pr.PullRequest.Url
    );

    var url = await Connection.Run(x);
    Logger.Info("Url " + url);
    return null;
  }

  public class PullRequest
  {
    public string? Body { get; init; }
    public string? Url { get; init; }
    public int Number { get; init; }
  }

  public class Repository
  {
    public ID? RepositoryId { get; init; }
    public string? DefaultBranchRef { get; init; }
    public string? DefaultBranchOid { get; init; }
  }
}