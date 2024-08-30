using Google.Protobuf;
using Microsoft.Extensions.Logging;
using System.Reflection;
using YYHEggEgg.EasyProtobuf.Configuration;
using YYHEggEgg.Logger;
using YYHEggEgg.Shell;
using YYHEggEgg.Shell.MainCLI;
using EggLogLevel = YYHEggEgg.Logger.LogLevel;

namespace YYHEggEgg.EasyProtobuf.MainCLI;

internal sealed class EasyProtobufCLI : AutoScanMainCommandLine
{
    public static readonly string? TargetProtobufVersion = Environment.GetEnvironmentVariable("EASYPROTOBUF_PROTOCOL_VERSION");

    protected override IEnumerable<string>? StartNewCommandNotices =>
        [
            $"----------New Work (Protobuf version: {TargetProtobufVersion})----------",
            "Type the proto name or command here; 'help' for commands help."
        ];

    public const string HistoryFileName = ".bash_history";
    protected override string? CommandHistoryFilePath => 
        Config.Global.RestoreHistoryOnRestart ? $"{HistoryFileName}{(string.IsNullOrEmpty(TargetProtobufVersion) ? "" : $".{TargetProtobufVersion}")}" : null;

    private ProtobufHandler _protobufOpHandler = new();
    public readonly BaseLogger? CommandHistoryLogger;

    public EasyProtobufCLI()
    {
        if (Config.Global.EnableRecordCommandHistory)
        {
            var conf = Log.GlobalConfig;
            conf.Console_Minimum_LogLevel = EggLogLevel.None;
            CommandHistoryLogger = new(conf, new LogFileConfig
            {
                MinimumLogLevel = EggLogLevel.Information,
                MaximumLogLevel = EggLogLevel.Information,
                AutoFlushWriter = true,
                FileIdentifier = "command-history",
                IsPipeSeparatedFile = true,
                AllowAutoFallback = true,
            });
        }
    }

    protected override CommandHandlerBase? RefreshGeneralOperationHandler()
    {
        return _protobufOpHandler;
    }

    protected override void RequestedExecuteCallback(string commandString)
    {
        CommandHistoryLogger?.Info(commandString, "InputCommand");
    }

    public override void Shutdown()
    {
        _logger.LogInformation("Thanks for using EasyProtobuf!");
        base.Shutdown();
    }
}
