using Scriban;
using Autofac;
using Tomlyn;

// Dependency injection
var builder = new ContainerBuilder();
builder.Register(c => Configuration.ParseConfiguration(Constants.CONFIGURATION_PATH)).As<Configuration>();
builder.RegisterInstance(new ActionContext()).As<ActionContext>();
builder.RegisterType<GitHub>().As<IGitHub>();

// Parse provided action on the command line (case insensitive)
var action = args.Length > 0 && Enum.TryParse(args[0], true, out Action actionValue) ? actionValue : Action.Error;
switch (action)
{
  case Action.Create:
    builder.RegisterType<Create>().As<ICommand>();

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
  case Action.Remind:
    throw new NotImplementedException("Not there yet!");
  default:
    Console.WriteLine("Error");
    break;
}

// Await for the command
await builder.Build().Resolve<ICommand>().Run();

// Available actions
public enum Action
{
  // Create a weekly draft
  Create,
  // Remind team about an existing weekly draft published as a Pull Request
  Remind,

  // Invalid action
  Error
}