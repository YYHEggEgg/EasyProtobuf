using AssetLib.Formats;
using YYHEggEgg.EasyProtobuf.Configuration;
using YYHEggEgg.Logger;
using YYHEggEgg.EasyProtobuf.Util;
using XC.RSAUtil;

namespace YYHEggEgg.EasyProtobuf.resLoader;

public static class ResourcesLoader
{
    public static string StructureDescription =>
        $"Resources Description\n" +
        $"/resources\n" +
        $"    /{Config.Global.RSAKeysDirectoryName}\n" +
        $"        /ClientPri -- Client Private Keys, CPri\n" +
        $"            /2.pem, ..., 5.pem -- RSA keys with key_id\n" +
        $"        /ServerPri-Hosting -- Server Private Keys (For Hosting like PS), SPri\n" +
        $"            /2-pri.pem, ..., 5-pri.pem -- RSA keys with key_id\n" +
        $"        /ServerPub-Official -- Serve Public Keys (For transfering to official server), SPub\n" +
        $"            /2-pub.xml, ..., 5-pub.xml -- RSA keys with key_id\n" +
        $"    /config-schemas\n" +
        $"        /config_schema_{{version}}.json -- schema json, DO NOT delete\n";

    private static LoggerChannel? _checklogger = null;

    #region Check
    /// <summary>
    /// Check for resources, if not complete then exit with code 114514.
    /// </summary>
    public static void CheckForRequiredResources(string resPath = "./resources")
    {
        _checklogger = Log.GetChannel("ResourcesCheck");
        bool passcheck = true;
        // Resources
        if (!Directory.Exists(resPath))
        {
            _checklogger.LogErro("resources dir missing! Please copy it from \"/resources\"!");
            _checklogger.LogInfo(StructureDescription);
            passcheck = false;
        }
        else
        {
            var rsakeys_basedir = Config.Global.RSAKeysDirectoryName;
            bool resourcesComplete = true;
            CheckDirectoryResource($"{rsakeys_basedir}/ClientPri", resPath, ref resourcesComplete);
            CheckDirectoryResource($"{rsakeys_basedir}/ServerPri-Hosting", resPath, ref resourcesComplete);
            CheckDirectoryResource($"{rsakeys_basedir}/ServerPub-Official", resPath, ref resourcesComplete);
            CheckDirectoryResource("config-schemas", resPath, ref resourcesComplete,
                continueOnSuccess: () =>
                {
                    foreach (var supportedVer in Config.SupportedVersions)
                    {
                        CheckFileResource($"config-schemas/config_schema_" +
                            $"v{supportedVer}.json", resPath, ref resourcesComplete);
                    }
                });
            if (!resourcesComplete)
            {
                _checklogger.LogInfo(StructureDescription);
                passcheck = false;
            }
        }
        if (!passcheck)
        {
            _checklogger.LogErro("Resources check didn't pass. Press Enter to exit.");
            if (Log.GlobalConfig.Use_Console_Wrapper) ConsoleWrapper.ReadLine();
            else Console.ReadLine();
            Environment.Exit(114514);
        }
    }

    private static void CheckFileResource(string path, string resBasePath, ref bool isResComplete)
    {
        var filePath = Path.Combine(resBasePath, path);
        if (!File.Exists(filePath))
        {
            _checklogger?.LogErro($"{filePath} not found!");
            isResComplete = false;
        }
    }

    private static void CheckDirectoryResource(string path, string resBasePath,
        ref bool isResComplete, Action? continueOnSuccess = null, Action? continueOnFailure = null)
    {
        var dirPath = Path.Combine(resBasePath, path);
        if (!Directory.Exists(dirPath))
        {
            _checklogger?.LogErro($"{dirPath} not found!");
            isResComplete = false;
            continueOnFailure?.Invoke();
        }
        else continueOnSuccess?.Invoke();
    }
    #endregion

    #region Load
    /// <summary>
    /// Load resources to Resources Class.
    /// </summary>
    public static async Task Load(string resPath = "./resources")
    {
        #region RSAKeys
        if (Directory.Exists($"{resPath}/rsakeys/ClientPri"))
        {
            foreach (var file in Directory.GetFiles($"{resPath}/rsakeys/ClientPri"))
            {
                FileInfo info = new(file);
                if (info.Extension != ".pem" && info.Extension != ".xml") continue;
                var name = info.Name;
                uint id = UInt32.Parse(name[0..name.IndexOf('-')]);
                try
                {
                    string pemKey = await File.ReadAllTextAsync(file);
                    Resources.CPri.Add(id, RSAUtilBase.LoadRSAKey(pemKey));
                }
                catch (Exception ex)
                {
                    LogTrace.WarnTrace(ex, nameof(ResourcesLoader), $"Load ClientPri key id: {id} failed, skipped file: {file}. ");
                }
            }
        }
        if (Directory.Exists($"{resPath}/rsakeys/ServerPub-Official"))
        {
            foreach (var file in Directory.GetFiles($"{resPath}/rsakeys/ServerPub-Official"))
            {
                FileInfo info = new(file);
                if (info.Extension != ".pem" && info.Extension != ".xml") continue;
                var name = info.Name;
                uint id = UInt32.Parse(name[0..name.IndexOf('-')]);
                try
                {
                    string pemKey = await File.ReadAllTextAsync(file);
                    Resources.OfficialSPub.Add(id, RSAUtilBase.LoadRSAKey(pemKey));
                }
                catch (Exception ex)
                {
                    LogTrace.WarnTrace(ex, nameof(ResourcesLoader),
                        $"Load ServerPub-Official key id: {id} failed, skipped file: {file}. ");
                }
            }
        }
        if (Directory.Exists($"{resPath}/rsakeys/ServerPri-Hosting"))
        {
            foreach (var file in Directory.GetFiles($"{resPath}/rsakeys/ServerPri-Hosting"))
            {
                FileInfo info = new(file);
                if (info.Extension != ".pem" && info.Extension != ".xml") continue;
                var name = info.Name;
                uint id = UInt32.Parse(name[0..name.IndexOf('-')]);
                try
                {
                    string pemKey = await File.ReadAllTextAsync(file);
                    Resources.LocalSPri.Add(id, RSAUtilBase.LoadRSAKey(pemKey));
                }
                catch (Exception ex)
                {
                    LogTrace.WarnTrace(ex, nameof(ResourcesLoader),
                        $"Load ServerPri-Hosting key id: {id} failed, skipped file: {file}. ");
                }
            }
        }
        #endregion

        Resources.BasePath = resPath;
    }
    #endregion

    private static bool IsBytesEqual(byte[]? l, byte[]? r)
    {
        if (l == null && r == null) return true;
        else if (l == null || r == null) return false;

        if (l.Length != r.Length) return false;
        for (int i = 0; i < l.Length; i++)
        {
            if (l[i] != r[i]) return false;
        }
        return true;
    }
}
