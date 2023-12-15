using CommandLine;
using Google.Protobuf;
using System.Reflection;
using System.Text.RegularExpressions;
using YYHEggEgg.EasyProtobuf.Commands;
using YYHEggEgg.EasyProtobuf.Configuration;
using YYHEggEgg.EasyProtobuf.resLoader;
using YYHEggEgg.EasyProtobuf.Util;
using YYHEggEgg.Logger;

namespace YYHEggEgg.EasyProtobuf;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
class ProtobufOption
{
    [Value(0, Required = true)]
    public string Protoname { get; set; }
}
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

internal class EasyProtobufProgram : StandardCommandHandler<ProtobufOption>
{
    public override string CommandName => nameof(EasyProtobufProgram);
    public override string Usage => throw new NotImplementedException();
    public override string Description => $"(Default) Type proto name and do operations.";
    public readonly static string? protobuf_version = Environment.GetEnvironmentVariable("EASYPROTOBUF_PROTOCOL_VERSION");

    static async Task Main(string[] args)
    {
        Log.Initialize(new LoggerConfig(
            max_Output_Char_Count: -1,
            use_Console_Wrapper: true,
            use_Working_Directory: true,
            debug_LogWriter_AutoFlush: true
        ));
        ConsoleWrapper.ShutDownRequest += Tools.ExitOnLaunching;
        string? version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3);
        Log.Info($"Welcome to EasyProtobuf v{version ?? "<unknown>"}! Protobuf version: {protobuf_version}.");

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

        await Start();
    }

    public override async Task HandleAsync(ProtobufOption opt)
    {
        var conf = Config.Global.EasyProtobufProgram;
        var protoname = opt.Protoname;
        Type? prototype = null;
        try
        {
            var protoNamespace = conf?.ProtoRootNamespace;
            if (protoNamespace == null) prototype = Type.GetType(protoname);
            else prototype = Type.GetType($"{protoNamespace}.{protoname}");
        }
        catch (Exception ex)
        {
            _logger.LogErroTrace(ex, $"Find Proto (by name) failed. ");
        }
        if (prototype == null)
        {
            _logger.LogErro($"Proto type or command not found: '{protoname}'.");
            return;
        }
        _logger.LogInfo("Well done! The proto exists.");

        _logger.LogInfo("Please type base64 encoded or HEX protobuf bin data (auto detect):");
        _logger.LogInfo("You can also paste json data to get its serialized data.");

        _logger.LogInfo(EasyInput.MultipleInputNotice);
        var raw_text = await ConsoleWrapper.ReadLineAsync(false);
        var res = EasyInput.TryPreProcess(raw_text);

        string? stroutput = null;
        IMessage? msg = null;
        switch (res.InputType)
        {
            case EasyInputType.Base64:
            case EasyInputType.Hex:
                var bytes = res.ToByteArray();
                msg = ProtoSerialize.Deserialize(prototype, bytes);
                if (msg == null)
                {
                    _logger.LogInfo($"Deserialized message = null");
                    _logger.LogWarn("Serialization/Deserialization probably failed!");
                    return;
                }
                stroutput = JsonFormatter.Default.Format(msg);
                _logger.LogInfo($"Converted Json:{Environment.NewLine}{stroutput}");
                break;
            case EasyInputType.Json:
                msg = ProtoSerialize.Serialize(prototype, res.ProcessedString ?? string.Empty);

                if (msg == null)
                {
                    _logger.LogInfo($"Serialized message = null");
                    _logger.LogWarn("Serialization/Deserialization probably failed!");
                    return;
                }
                stroutput = Convert.ToBase64String(msg.ToByteArray());
                _logger.LogInfo($"Serialized Base64:{Environment.NewLine}{stroutput}");
                break;
            default:
                _logger.LogErro($"Can't recognize the input type! Please try to specify the input type manually and try again.");
                break;
        }

        if (stroutput == null || msg == null)
        {
            _logger.LogWarn("Serialization/Deserialization probably failed!");
        }
        else
        {
            await Tools.SetClipBoardAsync(stroutput);

            var unksize = Tools.GetUnknownFieldsSize(msg, prototype);
            if (unksize != 0)
            {
                _logger.LogWarn($"Message has unknown fields that aren't defined in your proto: {unksize}/{msg.CalculateSize()} bytes. Please go to protobuf decode-raw tools for more information.");
            }
        }
    }

    private static List<CommandHandlerBase> ConfigureCommands()
    {
        var handlers = new List<CommandHandlerBase>()
        {
            new ConvertCmd(),
            new DecryptCurrRegionCmd(),
            new GenerateCurrRegionCmd(),
            new Ec2bCmd(),
            new MT19937Cmd(),
            new RsaKeyConvertCmd(),
            new XorCmd(),
        };
        return handlers;
    }

    static EasyProtobufProgram()
    {
        // StopProgram is now built-in command
        stopProgram = new StopCommand();
        handlers.Add(stopProgram);
        var cmdlist = ConfigureCommands();
        handlers.AddRange(cmdlist);
        protobufWorker = new();
    }

    private static StopCommand stopProgram;
    private static EasyProtobufProgram protobufWorker;
    public static List<CommandHandlerBase> handlers = new();
    public static void ShowHelps()
    {
        foreach (var handler in handlers)
        {
            handler.ShowDescription();
        }
        Log.Info("Type [command] help to get more detailed usage.", nameof(EasyProtobufProgram));
    }

    private static void RefuseCommand(string commandName)
    {
        Log.Info($"Invalid command: {commandName}.", nameof(EasyProtobufProgram));
    }

    public static async Task Start()
    {
        bool running = true;
        ConsoleWrapper.ShutDownRequest -= Tools.ExitOnLaunching;
        ConsoleWrapper.ShutDownRequest += async (_, _) =>
        {
            running = false;
            ConsoleWrapper.InputPrefix = string.Empty;
            await stopProgram.HandleAsync(string.Empty);
        };
        var helpstrings = CommandHandlerBase.HelpStrings;
        while (running)
        {
            ConsoleWrapper.InputPrefix = "> ";
            Log.Info($"----------New Work (Protobuf version: {protobuf_version})----------");
            Log.Info("Type the proto name or command here; 'help' for commands help.");
            string? cmd = ConsoleWrapper.ReadLine();
            if (string.IsNullOrEmpty(cmd))
            {
                continue;
            }
            int sepindex = cmd.IndexOf(' ');
            if (sepindex == -1) sepindex = cmd.Length;
            string commandName = cmd.Substring(0, sepindex);
            if (helpstrings.Contains(commandName.ToLower()))
            {
                ShowHelps();
                continue;
            }

            string argList = cmd.Substring(Math.Min(cmd.Length, sepindex + 1));
            var cmdhandle = (from handle in handlers
                             where handle.CommandName == commandName
                             select handle).FirstOrDefault();

            try
            {
                if (helpstrings.Contains(argList.Trim().ToLower()))
                {
                    if (cmdhandle == null) RefuseCommand(commandName);
                    else cmdhandle.ShowUsage();
                }
                else
                {
                    if (cmdhandle == null) await protobufWorker.HandleAsync(cmd); // fallback
                    else await cmdhandle.HandleAsync(argList);
                }
            }
            catch (Exception ex)
            {
                LogTrace.ErroTrace(ex,
                    prompt: $"Encountered error when handling command '{commandName}'. Please check your input. ");
            }

        }
        await Task.Delay(10000);
    }
}
