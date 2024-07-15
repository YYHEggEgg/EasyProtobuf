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
    private List<string>? _protoNames;
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

    protected override IEnumerable<string>? GetAllowedGeneralOperations()
    {
        if (_protoNames == null)
        {
            var conf = Config.Global.EasyProtobufProgram;
            var protoNamespace = conf?.ProtoRootNamespace;
            _protoNames = (from type in Assembly.GetExecutingAssembly().GetTypes()
                           where type.IsAssignableTo(typeof(IMessage))
                           where type.FullName != null && (protoNamespace == null || type.FullName.StartsWith(protoNamespace) == true)
                           select type.FullName![(protoNamespace == null ? 0 : protoNamespace.Length + 1)..]).ToList();
        }
        return _protoNames;
    }

    protected override CommandHandlerBase? GetGeneralOperationHandler()
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
