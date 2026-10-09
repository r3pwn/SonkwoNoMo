using System.Runtime.InteropServices;

namespace SonkwoNoMo.Client.Logging;

internal partial class ConsoleLogger : ILogger
{
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AllocConsole();

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool FreeConsole();

    [LibraryImport("kernel32.dll", EntryPoint = "SetConsoleTitleW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetConsoleTitle(
        [MarshalAs(UnmanagedType.LPWStr)] string title);

    [LibraryImport("kernel32.dll", EntryPoint = "OutputDebugStringW")]
    private static partial void OutputDebugString(
        [MarshalAs(UnmanagedType.LPWStr)] string message);

    public void Start()
    {
        if (AllocConsole())
        {
            SetConsoleTitle("SonkwoNoMo - Client");
        }
    }

    public void Stop()
    {
        FreeConsole();
    }

    public void Log(string message)
    {
        OutputDebugString(message);
        Console.WriteLine(message);
    }
}