using SonkwoSniffer;
using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!");
// NativeMethods.SkInit();
// Console.WriteLine("sk_init()");
NativeMethods.SkSetGamePath(@"C:\Program Files (x86)\Steam\steamapps\common\Laser League\GameProject\Binaries\Win64\GameProject-Win64-Shipping.exe");
Console.WriteLine("sk_set_game_path()");

//Console.WriteLine($"Current PID: {Environment.ProcessId}");
var steamInitResult = NativeMethods.SkSteamInit(
    "--REDACTED--", 
    "570460",
    "76561198119790993", 
    "2.0.0", 
    0);

Console.WriteLine($"sk_steam_init(): {steamInitResult}");

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

static unsafe void Handle30(nint p)
{
    Console.WriteLine("    [0x30] - No payload");
}

static unsafe void Handle37(nint p)
{
    var count = *(uint*)p;

    Console.WriteLine($"    server_count: {count}");

    var cursor = (byte*)p + sizeof(uint);

    for (uint i = 0; i < count; i++)
    {
        var serverName = Marshal.PtrToStringAnsi((nint)cursor);

        Console.WriteLine(
            $"      server[{i}]: {serverName}");

        // Advance past the string and its null terminator.
        while (*cursor != 0)
            cursor++;

        cursor++;
    }

    Hexdump(p, (int)(cursor - (byte*)p));
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