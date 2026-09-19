using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace SonkwoNoMo.Server.Services;

public sealed class AccessService(ILogger<AccessService> logger) : BackgroundService
{
    private const int Port = 4444;
    private TcpListener? _listener;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _listener = new TcpListener(IPAddress.Any, Port);
        _listener.Start();

        logger.LogInformation(
            $"Access TCP server listening on 0.0.0.0:{Port}");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var client = await _listener.AcceptTcpClientAsync(stoppingToken);

                _ = HandleClientAsync(client, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown.
        }
        finally
        {
            _listener.Stop();
        }
    }

    private static async Task HandleClientAsync(TcpClient client, CancellationToken ct)
    {
        var endpoint = client.Client.RemoteEndPoint;
        Console.WriteLine($"TCP client connected: {endpoint}");

        using (client)
        await using (var stream = client.GetStream())
        {
            while (true)
            {
                var header = await ReadExactly(stream, 8);

                if (header == null)
                    break;

                var length = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(0, 4));
                var command = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4, 4));

                Console.WriteLine(
                    $"TCP packet: length={length}, command={command} (0x{command:X})");

                if (length is < 8 or > 1024 * 1024)
                {
                    Console.WriteLine("Invalid packet length.");
                    break;
                }

                var payload = await ReadExactly(stream, checked((int)length - 8));

                if (payload == null)
                    break;

                Console.WriteLine(
                    $"Payload ({payload.Length} bytes): " +
                    Convert.ToHexString(payload));

                var value = MessagePack.MessagePackSerializer.Deserialize<object>(payload, cancellationToken: ct);

                Console.WriteLine(JsonSerializer.Serialize(value));

                if (command != 26)
                {
                    return;
                }

                // I have no idea why this does not work
                var response = new Dictionary<string, object?>
                {
                    ["op"] = 3,
                    ["sop"] = 0,
                    ["para_list"] = new List<object>
                    {
                        new Dictionary<string, object>
                        {
                            ["key"] = "ret",
                            ["type"] = 1,
                            ["ival"] = 1
                        }
                    }
                };

                var responsePayload =
                    MessagePack.MessagePackSerializer.Serialize(response);

                await SendPacket(stream, 27, responsePayload, ct);
            }
        }

        Console.WriteLine($"TCP client disconnected: {endpoint}");
    }

    private static async Task<byte[]?> ReadExactly(
        NetworkStream stream,
        int count)
    {
        var buffer = new byte[count];
        var offset = 0;

        while (offset < count)
        {
            var read = await stream.ReadAsync(
                buffer.AsMemory(offset, count - offset));

            if (read == 0)
                return null;

            offset += read;
        }

        return buffer;
    }

    private static async Task SendPacket(
        NetworkStream stream,
        uint command,
        byte[] payload,
        CancellationToken ct)
    {
        var header = new byte[8];

        BinaryPrimitives.WriteUInt32LittleEndian(
            header.AsSpan(0, 4),
            checked((uint)(8 + payload.Length)));

        BinaryPrimitives.WriteUInt32LittleEndian(
            header.AsSpan(4, 4),
            command);


        Console.WriteLine(
            $"Sending response: {Convert.ToHexString(header)}{Convert.ToHexString(payload)}"
        );

        await stream.WriteAsync(header, ct);
        await stream.WriteAsync(payload, ct);

        await stream.FlushAsync(ct);
    }
}
