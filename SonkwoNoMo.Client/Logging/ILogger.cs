namespace SonkwoNoMo.Client.Logging;

internal interface ILogger
{
    public void Start();

    public void Stop();

    public void Log(string message);
}