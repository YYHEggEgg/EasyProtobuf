using Newtonsoft.Json;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
namespace YYHEggEgg.EasyProtobuf.Configuration;

public class Config_v1_1_0
{
    public const string CORRESPONDING_VERSION = "1.1.0";

    public string ConfigVersion { get; set; }
    public bool EnableRecordCommandHistory { get; set; }
    public bool RestoreHistoryOnRestart { get; set; }
    public string RSAKeysDirectoryName { get; set; }
    public ProtobufCmdConfig_v1_0_0? EasyProtobufProgram { get; set; }
    public CurrRegionCmdsConfig_v1_0_0 CurrRegionCmds { get; set; }

    public static Config_v1_1_0 Deserialize(string json, string configVersion)
    {
        if (configVersion == CORRESPONDING_VERSION)
        {
            return JsonConvert.DeserializeObject<Config_v1_1_0>(json)
                ?? throw new JsonException("Config serialization failed: " +
                "please check whether your config matches the json format.");
        }
        else
        {
            return ParseOldVersion(json, configVersion);
        }
    }

    public static Config_v1_1_0 ParseOldVersion(string json, string configVersion)
    {
        if (configVersion == CORRESPONDING_VERSION)
            return Deserialize(json, configVersion);

        var olderconfig = Config_v1_0_0.ParseOldVersion(json, configVersion);
        return new Config_v1_1_0
        {
            ConfigVersion = configVersion,
            EnableRecordCommandHistory = olderconfig.EnableRecordCommandHistory,
            RestoreHistoryOnRestart = true,
            RSAKeysDirectoryName = olderconfig.RSAKeysDirectoryName,
            EasyProtobufProgram = olderconfig.EasyProtobufProgram,
            CurrRegionCmds = olderconfig.CurrRegionCmds,

        };
    }
}
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
