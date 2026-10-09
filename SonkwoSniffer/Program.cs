using SonkwoSniffer;
using System.Runtime.InteropServices;

NativeMethods.SkSetGamePath(@"C:\Program Files (x86)\Steam\steamapps\common\Laser League\GameProject\Binaries\Win64\GameProject-Win64-Shipping.exe");
Console.WriteLine("sk_set_game_path()");

var steamInitResult = NativeMethods.SkSteamInit(
    "--REDACTED--", 
    "570460",
    "76561198119790993", 
    "2.0.0", 
    0);

Console.WriteLine($"sk_steam_init(): {steamInitResult}");

for (var i = 0; i < 5; i++)
{
    Thread.Sleep(1000);
    unsafe
    {
        _ = NativeMethods.SkTick(0x12345, &HandleEvent);
    }
}

for (var i = 0; i < 30; i++)
{
    Thread.Sleep(1000);
    unsafe
    {
        _ = NativeMethods.SkTick(0x12345, &HandleEvent);
    }
}

return;

[UnmanagedCallersOnly]
static void HandleEvent(IntPtr ctx, ushort eventType, nint eventPtr)
{
    var header = Marshal.PtrToStructure<SkEventHeader>(eventPtr);

    Console.WriteLine("event_callback:");
    Console.WriteLine($"    context: 0x{ctx:X}");
    Console.WriteLine($"    callback type: {eventType}");
    Console.WriteLine($"    event Type: {header.Type}");
    Console.WriteLine($"    subtype: 0x{header.Subtype:X2}");
    Console.WriteLine($"    Field2: 0x{header.Field2:X8}");

    var payloadPtr = eventPtr + 0x8;
    switch (header.Subtype)
    {
        case 0x30:
            Handle30(payloadPtr);
            break;

        case 0x37:
            Handle37(payloadPtr);
            break;

        default:
            Hexdump(eventPtr, 0x100);
            break;
    }
}

static void Handle30(nint p)
{
    Console.WriteLine("    [0x30] - No payload");
}

static unsafe void Handle37(nint p)
{
    var count = *(uint*)p;
    var regionName = ReadAnsiString(p + 4);

    // p = eventPtr + 0x08
    // eventPtr + 0x110 = p + 0x108
    var regionTable = *(nint*)(p + 0x108);

    Console.WriteLine($"    available_regions: {count}");
    Console.WriteLine($"    selected_region: {regionName}");
    Console.WriteLine($"    region_table: 0x{regionTable:X}");

    if (regionTable == 0)
    {
        Console.WriteLine("    region_table: NULL");
        return;
    }

    for (var i = 0; i < count; i++)
    {
        var entry = regionTable + i * 0x10;

        var namePtr = *(nint*)entry;
        var valueA = *(uint*)(entry + 0x08);
        var valueB = *(uint*)(entry + 0x0C);

        var name = ReadAnsiString(namePtr);

        Console.WriteLine($"    region[{i}]:");
        Console.WriteLine($"        name:   \"{name}\"");
        Console.WriteLine($"        +0x08:  0x{valueA:X8} ({valueA})");
        Console.WriteLine($"        +0x0C:  0x{valueB:X8} ({valueB})");
    }
}

static unsafe string ReadAnsiString(nint address)
{
    if (address == 0)
        return "<NULL>";

    var ptr = (byte*)address;
    var length = 0;

    // Put a sanity limit on this so a bad pointer doesn't cause us
    // to walk arbitrary memory forever.
    const int maxLength = 0x1000;

    while (length < maxLength && ptr[length] != 0)
        length++;

    if (length == maxLength)
        return "<unterminated>";

    return Marshal.PtrToStringAnsi(address, length) ?? "<NULL>";
}

static unsafe void Hexdump(nint p, int length)
{
    for (var offset = 0; offset < length; offset += 0x10)
    {
        Console.Write($"    {offset:X4}: ");

        for (var i = 0; i < 0x10; i++)
        {
            var b = *((byte*)p + offset + i);
            Console.Write($"{b:X2} ");
        }

        Console.Write(" ");

        for (var i = 0; i < 0x10; i++)
        {
            var b = *((byte*)p + offset + i);
            Console.Write(b is >= 0x20 and <= 0x7E
                ? (char)b
                : '.');
        }

        Console.WriteLine();
    }
}