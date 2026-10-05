namespace SonkwoNoMo.Client.Utils.Logging;

public class NoopLogger : ILogger
{
    public void Start() { }

    public void Stop() { }

    public void Log(string _) { }
}