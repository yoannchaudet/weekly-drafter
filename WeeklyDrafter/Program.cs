using weekly_drafter;
using weekly_drafter.Commands;
using weekly_drafter.Services;

// Init the Actions context, parse the configuration and init a REST/GraphQL client
var actionContext = new ActionsContext();
var configurationPath = Path.Join(actionContext.GitHubWorkspace, Constants.ConfigurationPath);
var configuration = Configuration.ParseConfiguration(configurationPath);
var github = new GitHub(actionContext, configuration);

// Parse provided action on the command line (case insensitive)
string action = args.Length > 0 ? args[0].ToLowerInvariant() : "none provided";
switch (action)
{
  case "create":
    // Create a weekly update
    await new Create(actionContext, configuration, github).Run();
    break;
  case "validate":
    // Validate the provided configuration
    await new Validate(actionContext, configuration, github).Run();
    break;
  case "remind":
    throw new NotImplementedException("Not there yet!");
  default:
    throw Logger.Error($"Unsupported operation ({action})", throws: true)!;
}