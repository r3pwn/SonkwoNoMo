using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SonkwoNoMo
{
    public class EventQueue
    {
        private readonly ConcurrentQueue<nint> _queue = new();

        public unsafe void Enqueue(ushort type, ushort subtype, short reserved, object data)
        {
            var eventPtr = Marshal.AllocHGlobal(sizeof(SkEvent));

            var ev = (SkEvent*)eventPtr;
            ev->Type = type;
            ev->Subtype = subtype;
            ev->Field2 = reserved;
            ev->Payload = data;

            _queue.Enqueue(eventPtr);
        }

        public void Enqueue(SkEventType type, ushort subtype, short reserved, nint data) =>
            Enqueue((ushort)type, subtype, reserved, data);

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
}
