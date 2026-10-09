using SonkwoNoMo.Server.Access.Models;

namespace SonkwoNoMo.Server.Access.Handlers;

public static class AccessPacketHandler
{
    public static Task<AccessPacket?> HandleAsync(
        AccessPacket packet,
        CancellationToken ct)
    {
        return packet.Op switch
        {
            2 => HandleLoginRequestAsync(packet, ct),
            4 => HandleGameCheckRequestAsync(packet, ct),
            _ => HandleUnknownAsync(packet, ct)
        };
    }

    private static Task<AccessPacket?> HandleLoginRequestAsync(
        AccessPacket packet,
        CancellationToken ct)
    {
        var response = new AccessPacket
        {
            Op = 3,
            ParaList =
            [
                new Dictionary<string, object>
                {
                    ["key"] = "ret",
                    ["type"] = 1,
                    ["ival"] = 1
                }
            ]
        };

        return Task.FromResult<AccessPacket?>(response);
    }

    private static Task<AccessPacket?> HandleGameCheckRequestAsync(
        AccessPacket packet,
        CancellationToken ct)
    {
        var response = new AccessPacket
        {
            Op = 5,
            ParaList =
            [
                new Dictionary<string, object>
                {
                    ["key"] = "ret",
                    ["type"] = 1,
                    ["ival"] = 1
                }
            ]
        };

        return Task.FromResult<AccessPacket?>(response);
    }

    private static Task<AccessPacket?> HandleUnknownAsync(
        AccessPacket packet,
        CancellationToken ct)
    {
        return Task.FromResult<AccessPacket?>(null);
    }
}
