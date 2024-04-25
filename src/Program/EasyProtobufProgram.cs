using System.Reflection;
using YYHEggEgg.EasyProtobuf.Commands;
using YYHEggEgg.EasyProtobuf.Configuration;
using YYHEggEgg.EasyProtobuf.resLoader;
using YYHEggEgg.EasyProtobuf.Util;
using YYHEggEgg.Logger;

namespace YYHEggEgg.EasyProtobuf;

internal partial class EasyProtobufProgram : StandardCommandHandler<ProtobufOption>
{
    public readonly static string? protobuf_version = Environment.GetEnvironmentVariable("EASYPROTOBUF_PROTOCOL_VERSION");

    static async Task Main(string[] args)
    {
        Log.Initialize(new LoggerConfig(
            max_Output_Char_Count: -1,
            use_Console_Wrapper: true,
            use_Working_Directory: true,
            global_Minimum_LogLevel: LogLevel.Verbose,
            debug_LogWriter_AutoFlush: true
        ), CommandHistory.ReadSavedHistory());
        ConsoleWrapper.ShutDownRequest += Tools.ExitOnLaunching;
        string? version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3);
        Log.Info($"Welcome to EasyProtobuf v{version ?? "<unknown>"}! Protobuf version: {protobuf_version}.");
        Log.Info($"<3 from miHomo Software & YYHEggEgg");

        Tools.RunBackgroundUpdateCheck();

        #region Config
        bool configLoadSucc = true;
        var confPath = $"config-{protobuf_version}.json";
        var _conflog = Log.GetChannel("Configuration");
        try
        {
            _conflog.LogInfo($"Loading config...");
            await Config.InitializeAsync(confPath);
        }
        catch (Exception ex)
        {
            _conflog.LogWarnTrace(ex, $"Config: {confPath} initialize failed.");
            configLoadSucc = false;
        }

        var conf_validate_errs = await Config.ValidateAsync();
        if (conf_validate_errs != null && conf_validate_errs.Count > 0)
        {
            configLoadSucc = false;
            _conflog.LogWarn($"Validate '{confPath}' by schema failed. Detected errors below:");
            foreach (var err in conf_validate_errs)
            {
                _conflog.LogWarn(err.ToString());
            }
        }

        if (!configLoadSucc)
        {
            _conflog.LogErro("Config load failed. Please check the errors and fix them.");
            Environment.Exit(50);
        }
        #endregion

        ResourcesLoader.CheckForRequiredResources();
        await ResourcesLoader.Load();

        #region Command History
        if (Config.Global.EnableRecordCommandHistory)
        {
            var conf = Log.GlobalConfig;
            conf.Console_Minimum_LogLevel = LogLevel.None;
            CommandHistoryLogger = new(conf, new LogFileConfig
            {
                MinimumLogLevel = LogLevel.Information,
                MaximumLogLevel = LogLevel.Information,
                AutoFlushWriter = true,
                FileIdentifier = "command-history",
                IsPipeSeparatedFile = true,
                AllowAutoFallback = true,
            });
        }
        #endregion

        await Start();
    }
}
