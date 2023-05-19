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

    // var config = Configuration.ParseConfiguration("/Users/yoannchaudet/personal-repos/weekly-drafter/config.toml");
    // Console.WriteLine(config.Teams[0].Name);

    // var template = Template.ParseLiquid(File.ReadAllText("/Users/yoannchaudet/personal-repos/weekly-drafter/templates/weekly.content.liquid"));

    // var templateInputs = new
    // {
    //   config = config,
    //   mondayEnglish = Utils.GetMonday().ToEnglish(),
    //   mondaySortable = Utils.GetMonday().ToSortable()
    // };

    // var renderedTemplate = template.Render(templateInputs);
    // Console.WriteLine(renderedTemplate);

    break;
  case "remind":
    throw new NotImplementedException("Not there yet!");
  default:
    Console.WriteLine("Error");
    break;
}