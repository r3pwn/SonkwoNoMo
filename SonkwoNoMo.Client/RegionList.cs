using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SonkwoNoMo.Client;

public static unsafe class RegionList
{
    public static string SelectedRegion = string.Empty;
    public static Region[] Regions = [];

    public static async Task Refresh()
    {
        // TODO: pull actual server list
        HandleServerListResponse();
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

    public static SkPayload AsPayload()
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

    private static void WriteAscii(
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

    private static nint AllocAscii(string value)
    {
        var bytes = Encoding.ASCII.GetBytes(value + "\0");

        var memory = (byte*)NativeMemory.Alloc((nuint)bytes.Length);
        bytes.CopyTo(new Span<byte>(memory, bytes.Length));

        return (nint)memory;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct RegionEntry
{
    public nint Name;
    public uint PingMs;
    public uint ReqNum;
}

public record Region(
    string Name,
    string Host,
    uint PingMs,
    uint ReqNum);

