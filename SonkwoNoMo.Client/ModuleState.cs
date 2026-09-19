using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace SonkwoNoMo
{
    public enum SkEventType : ushort
    {
        Unknown = 0xFFFF,

        Event = 0,
        User = 5,
        Quit = 6,
        Broadcast = 7,
        Command = 8,
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct SkEvent
    {
        public ushort Type;
        public ushort Subtype;
        public short Field2;
        public object Payload;
    }

    public enum SkUserEventSubtype : ushort
    {
        NonAdult = 0,
        Adult = 1,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SkUserEventData
    {
        public short OnlineTime;
    }


    internal static class ModuleState
    {
        public static bool SteamInitialized = false;
        public static string GamePath = string.Empty;
        public static string AuthToken = string.Empty;
        public static string GameId = string.Empty;
        public static string BuildId = string.Empty;
        public static string GameVersion = string.Empty;
        public static string GameHost = string.Empty;
        public static ConcurrentQueue<nint> EventQueue = new();
        public static EventQueue Queue = new();
    }
}
