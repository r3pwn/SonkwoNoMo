using MessagePack;

namespace SonkwoNoMo.Server.Models;

[MessagePackObject]
public class AccessPacket
{
    [Key("op")]
    public int Op { get; set; } = 0;
    [Key("sop")]
    public int Sop { get; set; } = 0;
    [Key("user")]
    public string User { get; set; } = "";
    [Key("pro_id")]
    public int ProId { get; set; } = 0;
    [Key("g_id")]
    public int GId { get; set; } = 0;
    [Key("user_list")]
    public List<Dictionary<string, object>> UserList { get; set; } = [];
    [Key("para_list")]
    public List<Dictionary<string, object>> ParaList { get; set; } = [];
    [Key("data_list")]
    public List<Dictionary<string, object>> DataList { get; set; } = [];
    [Key("r_list")]
    public List<Dictionary<string, object>> RList { get; set; } = [];
}
