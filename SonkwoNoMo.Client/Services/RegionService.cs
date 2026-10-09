using System.Runtime.InteropServices;
using System.Text;
using SonkwoNoMo.Client.Core;
using SonkwoNoMo.Client.Interop;
using SonkwoNoMo.Client.Logging;

namespace SonkwoNoMo.Client.Services;

internal static class RegionService
{
    public static string SelectedRegion = string.Empty;
    public static Region[] Regions = [];

    public static async Task Refresh()
    {
        // TODO: pull actual server list
        HandleServerListResponse();
    }

    public static async Task RefreshAndQueueEventAsync()
    {
        try
        {
            await Refresh();

            ModuleState.Queue.Enqueue(
                7,
                0x37,
                0,
                AsPayload());
        }
        catch (Exception ex)
        {
            LoggerManager.Instance.Log(
                $"[Error] Region refresh failed: {ex}");
        }
    }

    private static void HandleServerListResponse()
    {
        // TODO: parse actual response
        Regions =
        [
            new Region("Offline", "invalid", 0, 25)
        ];

        if (string.IsNullOrEmpty(SelectedRegion) || Regions.FirstOrDefault(r => r.Name == SelectedRegion) == null)
        {
            SelectedRegion = Regions[0].Name;
        }
    }

    public static unsafe SkPayload AsPayload()
    {
        var payload = new SkPayload();

        // +0x00: region count
        *(uint*)payload.Raw = (uint)Regions.Length;

        // +0x04: default region string
        WriteAscii(payload.Raw + 4, 0x100 - 4, SelectedRegion);

        // +0x108: pointer to region table
        var table = (RegionEntry*)NativeMemory.Alloc(
            (nuint)(sizeof(RegionEntry) * Regions.Length));

        for (var i = 0; i < Regions.Length; i++)
        {
            var region = Regions[i];

            table[i].Name = AllocAscii(region.Name);
            table[i].PingMs = region.PingMs;
            table[i].ReqNum = region.ReqNum;
        }

        *(nint*)(payload.Raw + 0x108) = (nint)table;

        return payload;
    }

    private static unsafe void WriteAscii(
        byte* destination,
        int maxLength,
        string value)
    {
        var bytes = Encoding.ASCII.GetBytes(value);

        if (bytes.Length + 1 > maxLength)
            throw new ArgumentException("String is too long.");

        bytes.CopyTo(new Span<byte>(destination, bytes.Length));
        destination[bytes.Length] = 0;
    }

    private static unsafe nint AllocAscii(string value)
    {
        var bytes = Encoding.ASCII.GetBytes(value + "\0");

        var memory = (byte*)NativeMemory.Alloc((nuint)bytes.Length);
        bytes.CopyTo(new Span<byte>(memory, bytes.Length));

        return (nint)memory;
    }
}

internal record Region(
    string Name,
    string Host,
    uint PingMs,
    uint ReqNum);

