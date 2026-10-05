using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace SonkwoNoMo.Client
{

    [StructLayout(LayoutKind.Sequential)]
    public struct SkEvent
    {
        public ushort Type;
        public ushort Subtype;
        public uint Field2;
        public SkPayload Payload;
    }

    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public unsafe struct SkPayload
    {
        [FieldOffset(0)]
        public nint Pointer;

        [FieldOffset(0)]
        public uint Count;

        [FieldOffset(0)]
        public fixed byte Raw[32];
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
