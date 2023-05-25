using System.Text;
using Octokit;
using Octokit.GraphQL;
using Octokit.GraphQL.Model;
using weekly_drafter.Utils;
using Connection = Octokit.GraphQL.Connection;
using Label = Octokit.Label;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace weekly_drafter.Services;

public class GitHub
{
  public GitHub(ActionContext actionContext, Configuration configuration)
  {
    ActionContext = actionContext;
    Configuration = configuration;

    // Create a Rest client
    Rest = new GitHubClient(new ProductHeaderValue(Constants.GitHubAPIProductHeader))
    {
      Credentials = new Credentials(actionContext.GitHubToken)
    };

    // Create a GraphQL connection
    GraphQL = new Connection(new Octokit.GraphQL.ProductHeaderValue(Constants.GitHubAPIProductHeader),
      actionContext.GitHubToken);
  }

  private ActionContext ActionContext { get; }
  private Connection GraphQL { get; }
  private GitHubClient Rest { get; }
  private Configuration Configuration { get; }

  // Return the last few opened weekly update PRs
  public async Task<IEnumerable<PullRequest>> GetLastWeeklyUpdatePRs(int first = 10)
  {
    // PR filters
    var orderBy = new IssueOrder { Direction = OrderDirection.Desc, Field = IssueOrderField.CreatedAt };
    var labels = new[] { Constants.WeeklyUpdateLabel };
    var states = new[] { PullRequestState.Open };

    // Select last PRs still opened with the weekly-update label
    var query = new Query().Repository(ActionContext.GitHubRepositoryName, ActionContext.GitHubRepositoryOwnerName)
      .PullRequests(orderBy: orderBy, labels: labels, states: states, first: first).Nodes
      .Select(pr => new PullRequest { Body = pr.Body, Url = pr.Url, Number = pr.Number });
    return await GraphQL.Run(query);
  }

  // Return the current weekly update PR if any
  public async Task<PullRequest?> GetCurrentWeeklyUpdatePR(string sortableMonday)
  {
    return (await GetLastWeeklyUpdatePRs()).FirstOrDefault(pr => Markers.FromText(pr.Body!).Any(m =>
      m.Name == Constants.WeeklyUpdateMarker &&
      m.Arguments.Any(a => a.Key == Constants.WeeklyUpdateMarkerDate && a.Value == sortableMonday)));
  }

  // Return basic information about the current repository (e.g. default branch's name)
  public async Task<Repository> GetRepository()
  {
    var query = new Query().Repository(ActionContext.GitHubRepositoryName, ActionContext.GitHubRepositoryOwnerName)
      .DefaultBranchRef.Select(reference => new Repository
      {
        RepositoryId = reference.Repository.Id,
        DefaultBranchRef = reference.Name,
        DefaultBranchOid = reference.Target.Oid,
        Url = reference.Repository.Url
      });
    return await GraphQL.Run(query);
  }

  // Create or update the weekly-drafter label and return its object id
  private async Task<string> CreateOrUpdateLabel()
  {
    // Unfortunately we cannot use GraphQL for that, so switch to Rest
    using (Logger.Group($"Create or update {Constants.WeeklyUpdateLabel} label"))
    {
      // Check if a label already exists
      Label? existingLabel = null;
      try
      {
        Logger.Info("Try to fetch existing label");
        existingLabel = await Rest.Issue.Labels.Get(ActionContext.GitHubRepositoryOwnerName,
          ActionContext.GitHubRepositoryName, Constants.WeeklyUpdateLabel);
      }
      catch (NotFoundException)
      {
      }

      // Create label
      if (existingLabel == null)
      {
        Logger.Info("Create label");
        return (await Rest.Issue.Labels.Create(ActionContext.GitHubRepositoryOwnerName,
          ActionContext.GitHubRepositoryName,
          new NewLabel(Constants.WeeklyUpdateLabel, Constants.WeeklyUpdateLabelColor))).NodeId;
      }

      // Update label
      if (existingLabel.Color != Constants.WeeklyUpdateLabelColor)
      {
        Logger.Info("Update label");
        return (await Rest.Issue.Labels.Update(ActionContext.GitHubRepositoryOwnerName,
          ActionContext.GitHubRepositoryName, Constants.WeeklyUpdateLabel,
          new LabelUpdate(Constants.WeeklyUpdateLabel, Constants.WeeklyUpdateLabelColor))).NodeId;
      }

      // Keep existing label
      return existingLabel.NodeId;
    }
  }

  // Create a pull request for a given weekly update content
  public async Task<PullRequest> CreatePullRequest()
  {
    // Pull repository information
    var repository = await GetRepository();

    // Create or update the label in the background
    var createOrUpdateLabel = Task.Run(async () => await CreateOrUpdateLabel());

    // Create a new branch (starting at the default branch of the repository)
    // and fetch it's reference id
    var branchName = $"{Dates.GetMonday().ToSortable()}__{Guid.NewGuid().ToString()}";
    var branchRef = $"refs/heads/{branchName}";
    var createRef = new Mutation()
      .CreateRef(new CreateRefInput
      {
        RepositoryId = repository.RepositoryId!.Value, Name = branchRef, Oid = repository.DefaultBranchOid
      }).Select(reference => reference.Ref.Id);
    var branchRefId = await GraphQL.Run(createRef);

    // Shared template context
    var templatesContext = new
    {
      Config = Configuration,
      Date = Dates.GetMonday(),
      BranchName = branchName,
      RepoUrl = repository.Url
    };

    // Render the weekly update
    var contentPath = Path.Join(ActionContext.GitHubWorkspace, Constants.WeeklyTemplatePath);
    var content = Templates.RenderLiquidFromFile(contentPath, templatesContext);

    //  Add a commit on that new branch with the weekly update's content
    var createCommitOnBranch = new Mutation().CreateCommitOnBranch(new CreateCommitOnBranchInput
    {
      Branch = new CommittableBranch { Id = branchRefId },
      Message = new CommitMessage { Headline = "Add weekly update template" },
      ExpectedHeadOid = repository.DefaultBranchOid,
      FileChanges = new FileChanges
      {
        // Add the weekly update file, the content needs to be base64-encoded
        Additions = new[]
        {
          new FileAddition
          {
            Path = Configuration.RenderedWeeklyUpdatePath,
            Contents = Convert.ToBase64String(Encoding.UTF8.GetBytes(content))
          }
        },
        Deletions = new FileDeletion[] { }
      }
    }).Select(c => c.Commit.Oid);
    await GraphQL.Run(createCommitOnBranch);

    // Render the body of the PR
    var bodyPath = Path.Join(ActionContext.GitHubWorkspace, Constants.WeeklyPRTemplatePath);
    var body = new StringBuilder(Templates.RenderLiquidFromFile(bodyPath, templatesContext));
    body.AppendLine();
    body.AppendLine("<!-- Metadata, do not remove -->");
    body.AppendLine(Markers.ToText(
      new Markers.Marker(Constants.WeeklyUpdateMarker).AddArgument(Constants.WeeklyUpdateMarkerDate,
        templatesContext.Date.ToSortable())));

    // Create a pull request and return the object
    var createPullRequest = new Mutation().CreatePullRequest(new CreatePullRequestInput
    {
      RepositoryId = repository.RepositoryId!.Value,
      BaseRefName = repository.DefaultBranchRef,
      HeadRefName = branchRef,
      Title = $"Weekly update {Dates.GetMonday().ToSortable()}",
      Body = body.ToString(),
      MaintainerCanModify = true,
      Draft = false
    }).Select(pr => new PullRequest
    {
      Body = pr.PullRequest.Body,
      Url = pr.PullRequest.Url,
      Number = pr.PullRequest.Number,
      Id = pr.PullRequest.Id.Value
    });
    var pr = await GraphQL.Run(createPullRequest);
    var labelOid = createOrUpdateLabel.GetAwaiter().GetResult();
    var x = new Mutation().AddLabelsToLabelable(new AddLabelsToLabelableInput
    {
      LabelableId = new ID(pr.Id), LabelIds = new[] { new ID(labelOid) }
    }).Select(m => m.ClientMutationId);
    await GraphQL.Run(x);
    return pr;
  }

  public class PullRequest
  {
    public string? Body { get; init; }
    public string? Url { get; init; }
    public int Number { get; init; }
    public string? Id { get; init; }
  }

  public class Repository
  {
    public ID? RepositoryId { get; init; }
    public string? DefaultBranchRef { get; init; }
    public string? DefaultBranchOid { get; init; }
    public string? Url { get; init; }
  }
}