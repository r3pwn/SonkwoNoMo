using SonkwoSniffer;
using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!");
// NativeMethods.SkInit();
// Console.WriteLine("sk_init()");
NativeMethods.SkSetGamePath(@"C:\Program Files (x86)\Steam\steamapps\common\Laser League\GameProject\Binaries\Win64\GameProject-Win64-Shipping.exe");
Console.WriteLine("sk_set_game_path()");

//Console.WriteLine($"Current PID: {Environment.ProcessId}");
var steamInitResult = NativeMethods.SkSteamInit(
    "1400000058AF1C2E68012DBA91298209010010010130926A18000000010000000200000094B16DCA4DE05860240A030001000000B8000000380000000400000091298209010010015CB40800697F110801501FAC00000000CDB18C6A4D61A86A0100F134020001004ADD190000000000427D2CF352EFC23D12B944CB679E796D76B5F774D379609D22C1EDBD2E1A9F14BEDDCF5996C39517970FB5BA77D5BC2F1B932A6087118DDC98817CC84AE004C1B15B336B73E3EA42E2185C2FDBDD565FE03EA653218FA47EF53DEBA2970F6E8BEBDDEC920D4F632E0C680CD56BE6928F538E932AED0CAD3D1F345AB26B5E3C4E", 
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
static unsafe void HandleEvent(IntPtr ctx, ushort eventType, SkEvent* @event)
{
    Console.WriteLine("event_callback:");
    Console.WriteLine($"    context: {ctx}");
    Console.WriteLine($"    type: {eventType}");
    Console.WriteLine($"    subtype: {@event->Subtype}");
    Console.WriteLine($"    Field2: {@event->Field2}");
    Console.WriteLine($"    payload: {@event->Payload}");
}