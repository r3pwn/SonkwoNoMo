using System;
using System.IO;

namespace SonkwoNoMo;

public static class FileLogger
{
    private static readonly string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SonkwoNoMo_log.txt");
    private static readonly Lock LockObject = new Lock();

    public static void Log(string message)
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