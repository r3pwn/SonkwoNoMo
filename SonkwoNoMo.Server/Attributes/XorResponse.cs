using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using System.Text.Json;

namespace SonkwoNoMo.Server.Attributes;

public class XorResponseAttribute : Attribute, IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(
        ResultExecutingContext context,
        ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult)
        {
            var json = JsonSerializer.Serialize(objectResult.Value);
            var bytes = Encoding.UTF8.GetBytes(json);

            Xor(bytes);

            context.Result = new FileContentResult(
                bytes,
                "application/octet-stream");
        }

        await next();
    }

    private static void Xor(byte[] bytes)
    {
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] ^= (byte)((i % 7) + 1);
        }
    }
}