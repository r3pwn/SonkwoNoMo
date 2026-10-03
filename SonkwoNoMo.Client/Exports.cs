using System.Runtime.InteropServices;

namespace SonkwoNoMo.Client;

public static unsafe class Exports
{
    [UnmanagedCallersOnly(EntryPoint = "sk_tick")]
    public static uint SkTick(nint eventContext, delegate* unmanaged<nint, ushort, SkEvent*, void> eventCallback)
    {
        if (!ModuleState.SteamInitialized)
        {
            return 0xffffffff;
        }

        while (ModuleState.Queue.TryDequeue(out var ev))
        {
            FileLogger.Log($"[Log] sk_tick: executing callback for event with params {eventContext}:{ev->Type}:{ev->Subtype}:{ev->Field2}:{ev->Payload}");
            eventCallback(
                eventContext,
                ev->Type,
                ev);

            FreeEvent(ev);
        }

        return 0;
    }

    [UnmanagedCallersOnly(EntryPoint = "sk_steam_init")]
    public static uint SkSteamInit(byte* authToken, byte* gameId, byte* buildId, byte* gameVersion, int isTestBuild)
    {
        try
        {
            // Convert the raw native char* pointers to C# managed strings for logging
            var s1 = GetString(authToken);
            var s2 = GetString(gameId);
            var s3 = GetString(buildId);
            var s4 = GetString(gameVersion);

            FileLogger.Log($"[Log] sk_steam_init: '{s1}', '{s2}', '{s3}', '{s4}', {isTestBuild}");

            if (ModuleState.SteamInitialized)
            {
                return 0xfffffffb;
            }

            ModuleState.AuthToken = s1;
            ModuleState.GameId = s2;
            ModuleState.BuildId = s3;
            ModuleState.GameVersion = s4;
            ModuleState.SteamInitialized = true;

            ModuleState.Queue.Enqueue(
                7,
                0x30,
                1,
                new SkPayload { Pointer = IntPtr.Zero });

            return 0;
        }
        catch (Exception ex)
        {
            // UnmanagedCallersOnly methods MUST NOT let exceptions escape to native code
            FileLogger.Log($"[Error] Exception in export: {ex.Message}");
            return 0xffffffff;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "sk_set_game_path")]
    public static void SkSetGamePath(byte* gamePath)
    {
        var path = GetString(gamePath);
        FileLogger.Log($"[Log] sk_set_game_path: {path}");
        ModuleState.GamePath = path;
    }

    [UnmanagedCallersOnly(EntryPoint = "sk_init")]
    public static int SkInit()
    {
        FileLogger.Log($"[Log] sk_init");
        return 0;
    }

    [UnmanagedCallersOnly(EntryPoint = "sk_destroy")]
    public static void SkDestroy()
    {
        FileLogger.Log($"[Log] sk_destroy");
    }

    [UnmanagedCallersOnly(EntryPoint = "sk_get_region_list_ping_ttl")]
    public static int SkGetRegionListPingTtl()
    {
        FileLogger.Log($"[Log] sk_get_region_list_ping_ttl");
        return 1;
    }

    [UnmanagedCallersOnly(EntryPoint = "sk_get_overlay_status")]
    public static int SkGetOverlayStatus()
    {
        FileLogger.Log($"[Log] sk_get_overlay_status");
        return 0;
    }

    [UnmanagedCallersOnly(EntryPoint = "sk_select_region")]
    public static long SkSelectRegion(byte* region)
    {
        try
        {
            var regionStr = GetString(region);
            FileLogger.Log($"[Log] sk_select_region: {regionStr}");

            return 0;
        }
        catch { return -1; }
    }

    [UnmanagedCallersOnly(EntryPoint = "sk_get_user_info")]
    public static int SkGetUserInfo()
    {
        FileLogger.Log($"[Log] sk_get_user_info");
        return 0;
    }

    [UnmanagedCallersOnly(EntryPoint = "sk_get_version")]
    public static int SkGetVersion()
    {
        FileLogger.Log($"[Log] sk_get_version");
        return 0x10000;
    }

    [UnmanagedCallersOnly(EntryPoint = "sk_create_party")]
    public static int SkCreateParty()
    {
        FileLogger.Log($"[Log] sk_create_party");
        return 0;
    }

    private static string GetString(byte* charPtr)
    {
        return Marshal.PtrToStringAnsi((IntPtr)charPtr) ?? string.Empty;
    }

    private static void FreeEvent(SkEvent* eventPtr)
    {
        Marshal.FreeHGlobal((nint)eventPtr);
    }
}