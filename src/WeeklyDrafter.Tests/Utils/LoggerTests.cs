public class LoggerTests
{
  // CommandEnvelope test

  [Fact]
  public void CommandEnvelopeToString()
  {
    var envelope = new Logger.CommandEnvelope("test", new Dictionary<string, string> { { "foo", "bar" } }, "hello");
    Assert.Equal("::test foo=bar hello", envelope.ToString());
  }

  [Fact]
  public void CommandEnvelopeNoParametersToString()
  {
    var envelope = new Logger.CommandEnvelope("test", null, "hello");
    Assert.Equal("::test hello", envelope.ToString());
  }

  [Fact]
  public void CommandEnvelopeNoParametersNoMessageToString()
  {
    // probably invalid but we'll allow it
    var envelope = new Logger.CommandEnvelope("test", null, "");
    Assert.Equal("::test", envelope.ToString());
  }

  [Fact]
  public void CommandEnvelopeParametersEscapingToString()
  {
    var envelope = new Logger.CommandEnvelope("test", new Dictionary<string, string> { { "a", "b:" }, { "c", "d," } }, "\r\n%hello%");
    Assert.Equal("::test a=b%3A,c=d%2C %0D%0A%25hello%25", envelope.ToString());
  }
}