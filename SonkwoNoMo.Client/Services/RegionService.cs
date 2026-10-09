using System.Runtime.InteropServices;
using System.Text;
using SonkwoNoMo.Client.Core;
using SonkwoNoMo.Client.Interop;
using SonkwoNoMo.Client.Logging;

namespace SonkwoNoMo.Client.Services;

internal static class RegionService
{
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

    public static string SelectedRegion = string.Empty;

    public static async Task Refresh()
    {
        // TODO: call the region list REST API and build the staging list
        Region[] regions =
        [
            new("Offline", "invalid", 0, 25)
        ];

        // TODO: ping each region's host to determine the latency

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
}

internal record Region(
    string Name,
    string Host,
    uint PingMs,
    uint ReqNum);
