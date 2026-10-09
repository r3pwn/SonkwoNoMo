using SonkwoNoMo.Client.Core;

namespace SonkwoNoMo.Client.Logging;

internal class FileLogger : ILogger
{
    private static readonly string LogPath = Path.Combine(Paths.BasePath, "debug_log.txt");
    private static readonly Lock LockObject = new();

    public void Start() { }

    public void Stop() { }

    public void Log(string message)
    {
        try
        {
            lock (LockObject)
            {
                // Appends text and automatically manages a newline
                File.AppendAllText(LogPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
            }
        }
        catch
        {
            // Fail silently to prevent crashing the host application if the file is locked
        }
    }
}