using System.Text.Json.Serialization;

namespace SonkwoNoMo.Server.Models;

public class Region
{
    [JsonPropertyName("region")]
    public string Name { get; set; }
    [JsonPropertyName("request_num")]
    public int RequestNum { get; set; }
}