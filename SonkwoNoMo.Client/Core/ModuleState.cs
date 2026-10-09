namespace SonkwoNoMo.Client.Core;

internal static class ModuleState
{
    public static bool SteamInitialized = false;
    public static string GamePath = string.Empty;
    public static string AuthToken = string.Empty;
    public static string GameId = string.Empty;
    public static string BuildId = string.Empty;
    public static string GameVersion = string.Empty;

    public static EventQueue Queue = new();
}