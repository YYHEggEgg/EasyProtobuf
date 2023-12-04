using XC.RSAUtil;

namespace YYHEggEgg.EasyProtobuf.resLoader;

public static class Resources
{
    /// <summary>
    /// The path provided in the successful attempt of <see cref="ResourcesLoader.Load(string)"/>.
    /// </summary>
    public static string BasePath = "./resources";
    public static Dictionary<uint, RSAUtilBase> CPri = new();
    public static Dictionary<uint, RSAUtilBase> OfficialSPub = new();
    public static Dictionary<uint, RSAUtilBase> LocalSPri = new();
}
