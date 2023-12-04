using Newtonsoft.Json;
using YYHEggEgg.Logger;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
namespace YYHEggEgg.EasyProtobuf.Configuration;

public class ProtobufCmdConfig_v1_0_0
{
    public string? ProtoRootNamespace { get; set; }
}

public class CurrRegionCmdsConfig_v1_0_0
{
    public string? BaseProto { get; set; }
}

public class Config_v1_0_0
{
    public const string CORRESPONDING_VERSION = "1.0.0";

    public string ConfigVersion { get; set; }
    public string RSAKeysDirectoryName { get; set; }
    public ProtobufCmdConfig_v1_0_0? EasyProtobufProgram { get; set; }
    public CurrRegionCmdsConfig_v1_0_0 CurrRegionCmds { get; set; }

    public static Config_v1_0_0 Deserialize(string json, string configVersion)
    {
        if (configVersion == CORRESPONDING_VERSION)
        {
            return JsonConvert.DeserializeObject<Config_v1_0_0>(json)
                ?? throw new JsonException("Config serialization failed: " +
                "please check whether your config matches the json format.");
        }
        else
        {
            return ParseOldVersion(json, configVersion);
        }
    }

    public static Config_v1_0_0 ParseOldVersion(string json, string configVersion)
    {
        // Since 1.0.0 is the earliest version, we can just return the result of Deserialize.
        return Deserialize(json, CORRESPONDING_VERSION);
    }
}
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
