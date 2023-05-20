using weekly_drafter;
using weekly_drafter.Commands;
using weekly_drafter.Services;

var actionContext = new ActionContext();
var configurationPath = Path.Join(actionContext.GitHubWorkspace, Constants.ConfigurationPath);
var configuration = Configuration.ParseConfiguration(configurationPath);
var github = new GitHub(actionContext, configuration);

// Parse provided action on the command line (case insensitive)
var action = args.Length > 0 ? args[0].ToLowerInvariant() : null;
switch (action)
{
  case "create":
    await new Create(actionContext, configuration, github).Run();
    break;
  case "remind":
    throw new NotImplementedException("Not there yet!");
  default:
    Console.WriteLine("Error");
    break;
}