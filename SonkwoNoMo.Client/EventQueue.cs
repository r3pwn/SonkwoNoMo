using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace SonkwoNoMo.Client;

[StructLayout(LayoutKind.Sequential)]
public struct SkEvent
{
    public ushort Type;
    public ushort Subtype;
    public uint Field2;
    public SkPayload Payload;
}

[StructLayout(LayoutKind.Explicit, Size = 0x200)]
public unsafe struct SkPayload
{
    [FieldOffset(0)] public nint Pointer;

    [FieldOffset(0)] public fixed byte Raw[0x200];
}

public class EventQueue
{
    private readonly ConcurrentQueue<nint> _queue = new();

    public unsafe void Enqueue(ushort type, ushort subtype, uint reserved, SkPayload data)
    {
        var eventPtr = Marshal.AllocHGlobal(sizeof(SkEvent));

        var ev = (SkEvent*)eventPtr;
        ev->Type = type;
        ev->Subtype = subtype;
        ev->Field2 = reserved;
        ev->Payload = data;

        _queue.Enqueue(eventPtr);
    }

    public unsafe bool TryDequeue(out SkEvent* skEvent)
    {
        if (!_queue.TryDequeue(out var eventPtr) || eventPtr == 0)
        {
            skEvent = (SkEvent*)0;
            return false;
        }

        skEvent = (SkEvent*)eventPtr;

        return true;
    }
}