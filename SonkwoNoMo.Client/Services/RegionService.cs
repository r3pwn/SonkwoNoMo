using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using SonkwoNoMo.Client.Config;
using SonkwoNoMo.Client.Core;
using SonkwoNoMo.Client.Interop;
using SonkwoNoMo.Client.Logging;

namespace SonkwoNoMo.Client.Services;

internal static class RegionService
{
    public static string SelectedRegion = string.Empty;

    // The region table and name buffers are allocated once and updated in
    // place on each refresh. The game may read them at any time, so they
    // are never freed or reallocated (only at module shutdown). Writes are
    // bounded to fixed-size slots, 32-bit fields are atomic on x86, and
    // only string contents can transiently tear, which self-corrects on
    // the next frame.
    private const int MaxRegions = 16;
    private const int NameBufferSize = 256;

    private static readonly Lock BufferLock = new();

    private static unsafe byte* _table;
    private static unsafe byte* _nameBuffers;
    private static int _count;

    private static readonly HttpClient _client = new()
    {
        Timeout = TimeSpan.FromSeconds(5),
    };

    public static async Task Refresh()
    {
        var fetched = await FetchRegionsAsync();

        // Gateway unreachable or returned no usable data: keep the game
        // launchable with a local-only entry.
        var regions = fetched is null
            ? [new Region("Offline", "invalid", 0, 25)]
            : await PingRegionsAsync(fetched);

        if (regions.Length == 0)
        {
            // Keep the last known list rather than publishing an empty one.
            return;
        }

        if (string.IsNullOrEmpty(SelectedRegion) || regions.FirstOrDefault(r => r.Name == SelectedRegion) == null)
        {
            SelectedRegion = regions[0].Name;
        }

        Publish(regions);
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

    public static unsafe SkPayload AsPayload()
    {
        lock (BufferLock)
        {
            AllocBufferIfNeeded();

            var payload = new SkPayload();

            // +0x00: region count
            *(uint*)payload.Raw = (uint)_count;

            // +0x04: default region string
            WriteFixedAscii(payload.Raw + 4, 0x100 - 4, SelectedRegion);

            // +0x108: pointer to region table
            *(nint*)(payload.Raw + 0x108) = (nint)_table;

            return payload;
        }
    }

    public static unsafe void Shutdown()
    {
        lock (BufferLock)
        {
            if (_table != null)
            {
                NativeMemory.Free(_table);
                _table = null;
            }

            if (_nameBuffers != null)
            {
                NativeMemory.Free(_nameBuffers);
                _nameBuffers = null;
            }

            _count = 0;
        }
    }

    private static unsafe void Publish(Region[] regions)
    {
        lock (BufferLock)
        {
            AllocBufferIfNeeded();

            var count = Math.Min(regions.Length, MaxRegions);
            var table = (RegionEntry*)_table;

            for (var i = 0; i < count; i++)
            {
                WriteName(i, regions[i].Name);

                table[i].PingMs = regions[i].PingMs;
                table[i].ReqNum = regions[i].ReqNum;

                // Name pointer goes last: it marks the slot's data as in place.
                table[i].Name = (nint)(_nameBuffers + (i * NameBufferSize));
            }

            _count = count;
        }
    }

    private static unsafe void WriteName(int slot, string value)
    {
        var bytes = Encoding.ASCII.GetBytes(value);
        const int max = NameBufferSize - 1;

        if (bytes.Length > max)
        {
            bytes = bytes[..max];
        }

        var destination = _nameBuffers + (slot * NameBufferSize);
        bytes.CopyTo(new Span<byte>(destination, bytes.Length));
        destination[bytes.Length] = 0;
    }

    private static unsafe void WriteFixedAscii(
        byte* destination,
        int maxLength,
        string value)
    {
        var bytes = Encoding.ASCII.GetBytes(value);

        if (bytes.Length > maxLength)
        {
            bytes = bytes[..maxLength];
        }

        bytes.CopyTo(new Span<byte>(destination, bytes.Length));
        destination[bytes.Length] = 0;
    }

    private static unsafe void AllocBufferIfNeeded()
    {
        if (_table != null)
        {
            return;
        }

        _table = (byte*)NativeMemory.Alloc((nuint)(MaxRegions * Marshal.SizeOf<RegionEntry>()));
        _nameBuffers = (byte*)NativeMemory.Alloc(MaxRegions * NameBufferSize);
    }

    private static async Task<Region[]?> FetchRegionsAsync()
    {
        try
        {
            var gateway = ConfigLoader.Instance.Gateway;
            using var response = await _client.GetAsync(
                $"http://{gateway.Host}:{gateway.Port}/api/get_region_list");

            if (response is not { IsSuccessStatusCode: true })
            {
                return null;
            }

            var payload = await response.Content.ReadAsByteArrayAsync();
            XorInPlace(payload);

            using var document = JsonDocument.Parse(payload);
            var root = document.RootElement;

            var accInfo = root.TryGetProperty("acc_info", out var accInfoProp)
                ? accInfoProp.GetString()
                : null;

            if (root.TryGetProperty("all_region", out var allRegions) is false ||
                allRegions.ValueKind is not JsonValueKind.Array)
            {
                return null;
            }

            var regions = new List<Region>();

            foreach (var entry in allRegions.EnumerateArray())
            {
                var region = entry.ValueKind is JsonValueKind.Object
                    ? BuildRegion(entry, accInfo)
                    : null;

                if (region is not null)
                {
                    regions.Add(region);
                }
            }

            return regions.Count > 0 ? [.. regions] : null;
        }
        catch (Exception ex)
        {
            LoggerManager.Instance.Log($"[Warning] Region list fetch failed: {ex.Message}");
            return null;
        }
    }

    private static Region? BuildRegion(JsonElement entry, string? accInfo)
    {
        var raw = entry.TryGetProperty("region", out var regionProp)
            ? regionProp.GetString()
            : null;

        var reqNum = entry.TryGetProperty("request_num", out var reqNumProp)
            && reqNumProp.ValueKind is JsonValueKind.Number
            ? (uint)reqNumProp.GetInt32()
            : 25u;

        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        // Entries are "Name|Host:Port"; without a '|' the shared acc_info
        // endpoint is the host and the whole string is the display name.
        var separator = raw.LastIndexOf('|');
        var name = (separator >= 0 ? raw[..separator] : raw).Trim();
        var host = ((separator >= 0 ? raw[(separator + 1)..] : accInfo) ?? string.Empty).Trim();

        if (string.IsNullOrEmpty(host))
        {
            return null;
        }

        if (string.IsNullOrEmpty(name))
        {
            name = host;
        }

        return new Region(name, host, uint.MaxValue, reqNum);
    }

    private static async Task<Region[]> PingRegionsAsync(Region[] regions)
    {
        var pings = regions.Select(async region =>
        {
            var pingMs = await MeasurePingMsAsync(region.Host);
            return region with { PingMs = pingMs };
        });

        return await Task.WhenAll(pings);
    }

    private static async Task<uint> MeasurePingMsAsync(string hostPort)
    {
        // The ICMP echo goes to the host, so the port is stripped off.
        var separator = hostPort.LastIndexOf(':');
        var host = (separator >= 0 ? hostPort[..separator] : hostPort).Trim();

        if (string.IsNullOrEmpty(host))
        {
            return uint.MaxValue;
        }

        using var ping = new Ping();

        try
        {
            var reply = await ping.SendPingAsync(host, 2000);
            return reply.Status is IPStatus.Success
                ? (uint)reply.RoundtripTime
                : uint.MaxValue;
        }
        catch
        {
            return uint.MaxValue;
        }
    }

    // The gateway obfuscates responses with a rolling XOR; applying the same
    // operation again restores the payload.
    private static void XorInPlace(byte[] data)
    {
        for (var i = 0; i < data.Length; i++)
        {
            data[i] ^= (byte)((i % 7) + 1);
        }
    }
}

internal record Region(
    string Name,
    string Host,
    uint PingMs,
    uint ReqNum);
