namespace SonkwoNoMo.Client.Logging;

internal class NoopLogger : ILogger
{
    public void Start() { }

    public void Stop() { }

    public void Log(string _) { }
}