namespace SonkwoNoMo.Client.Utils.Logging;

public interface ILogger
{
    public void Start();

    public void Stop();

    public void Log(string message);
}