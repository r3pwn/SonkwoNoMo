using System.Runtime.InteropServices;

namespace SonkwoNoMo.Client.Interop;

[StructLayout(LayoutKind.Sequential)]
internal struct SkEvent
{
    public ushort Type;
    public ushort Subtype;
    public uint Field2;
    public SkPayload Payload;
}

[StructLayout(LayoutKind.Explicit, Size = 0x200)]
internal unsafe struct SkPayload
{
    [FieldOffset(0)] public nint Pointer;

    [FieldOffset(0)] public fixed byte Raw[0x200];
}
