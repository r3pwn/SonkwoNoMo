using System.Runtime.InteropServices;

namespace SonkwoNoMo.Client.Interop;

[StructLayout(LayoutKind.Sequential)]
internal struct RegionEntry
{
    public nint Name;
    public uint PingMs;
    public uint ReqNum;
}
