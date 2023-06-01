namespace weekly_drafter;

public class ActionsContext
{
  // Default variables from https://docs.github.com/en/actions/learn-github-actions/variables#default-environment-variables
  public string GitHubWorkspace => GetFromEnvironment("GITHUB_WORKSPACE", true)!;
  public string GitHubToken => GetFromEnvironment("GITHUB_TOKEN", true)!;
  public string GitHubRepository => GetFromEnvironment("GITHUB_REPOSITORY", true)!;
  public string GitHubRepositoryOwnerName => GitHubRepository.Split("/")[0];
  public string GitHubRepositoryName => GitHubRepository.Split("/")[1];

  // Extract a value from the environment variable.
  private static string? GetFromEnvironment(string key, bool required)
  {
    var value = Environment.GetEnvironmentVariable(key);
    if (value == null && required)
      throw Logger.Error($"Unable to extract required environment variable '{key}' from Actions context",
        throws: true)!;
    return value;
  }
}