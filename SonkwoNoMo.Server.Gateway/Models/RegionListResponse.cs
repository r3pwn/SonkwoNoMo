using System.Text.Json.Serialization;

namespace SonkwoNoMo.Server.Gateway.Models;

public class RegionListResponse
{
    [JsonPropertyName("acc_info")]
    public string AccInfo { get; set; }
    [JsonPropertyName("all_region")]
    public List<Region> Regions { get; set; }
}