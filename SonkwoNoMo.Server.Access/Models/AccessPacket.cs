using System.Text.Json.Serialization;
using MessagePack;

namespace SonkwoNoMo.Server.Access.Models;

[MessagePackObject]
public class AccessPacket
{
    [Key("op")]
    [JsonPropertyName("op")]
    public int Op { get; set; } = 0;
    [Key("sop")]
    [JsonPropertyName("sop")]
    public int Sop { get; set; } = 0;
    [Key("user")]
    [JsonPropertyName("user")]
    public string User { get; set; } = "";
    [Key("pro_id")]
    [JsonPropertyName("pro_id")]
    public int ProId { get; set; } = 0;
    [Key("g_id")]
    [JsonPropertyName("g_id")]
    public int GId { get; set; } = 0;
    [Key("user_list")]
    [JsonPropertyName("user_list")]
    public List<Dictionary<string, object>> UserList { get; set; } = [];
    [Key("para_list")]
    [JsonPropertyName("para_list")]
    public List<Dictionary<string, object>> ParaList { get; set; } = [];
    [Key("data_list")]
    [JsonPropertyName("data_list")]
    public List<Dictionary<string, object>> DataList { get; set; } = [];
    [Key("r_list")]
    [JsonPropertyName("r_list")]
    public List<Dictionary<string, object>> RList { get; set; } = [];
}
