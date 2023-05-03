public interface IGitHub
{
  public Task<IEnumerable<PullRequest>> GetLastWeeklyUpdatePRs(int first = 10);

  public Task<IGitHub.PullRequest?> GetCurrentWeeklyUpdatePR(string sortableMonday);

  public class PullRequest
  {
    public string? Body { get; set; }
    public string? Url { get; set; }
    public int Number { get; set; }
  }
}