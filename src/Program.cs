// Parse provided action on the command line (case insensitive)
var action = args.Length > 0 && Enum.TryParse(args[0], true, out Action actionValue) ? actionValue : Action.Error;
switch (action) {
  case Action.Create:
    Console.WriteLine("Create");
    break;
  case Action.Remind:
    throw new NotImplementedException("Not there yet!");
  default:
    Console.WriteLine("Error");
    break;
}

// Available actions
public enum Action {
  // Create a weekly draft
  Create,
  // Remind team about an existing weekly draft published as a Pull Request
  Remind,

  // Invalid action
  Error
}