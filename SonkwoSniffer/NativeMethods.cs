using System.Runtime.InteropServices;

namespace SonkwoSniffer;

[StructLayout(LayoutKind.Sequential)]
public struct SkEventHeader
{
    public ushort Type;
    public ushort Subtype;
    public uint Field2;
}

internal static partial class NativeMethods
{
    [LibraryImport("MMClientSDK.dll", EntryPoint = "sk_init")]
    internal static partial int SkInit();

    [LibraryImport("MMClientSDK.dll", EntryPoint = "sk_set_game_path", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void SkSetGamePath(string path);

    [LibraryImport("MMClientSDK.dll", EntryPoint = "sk_steam_init", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial uint SkSteamInit(
        string authToken,
        string gameId,
        string buildId,
        string gameVersion,
        int isTestBuild);

    [LibraryImport("MMClientSDK.dll", EntryPoint = "sk_select_region", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial uint SkSelectRegion(string region);

    [LibraryImport("MMClientSDK.dll", EntryPoint = "sk_tick")]
    internal static unsafe partial uint SkTick(
        nint eventContext,
        delegate* unmanaged<nint, ushort, nint, void> eventCallback);

    [LibraryImport("MMClientSDK.dll", EntryPoint = "sk_get_region_list_ping_ttl")]
    internal static unsafe partial uint SkGetRegionListPingTtl();
}