using CommandLine;
using Google.Protobuf;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using YYHEggEgg.EasyProtobuf.Commands;
using YYHEggEgg.EasyProtobuf.Configuration;
using YYHEggEgg.EasyProtobuf.resLoader;
using YYHEggEgg.EasyProtobuf.Util;
using YYHEggEgg.Logger;

namespace YYHEggEgg.EasyProtobuf;

internal partial class EasyProtobufProgram
{
    private static BaseLogger? CommandHistoryLogger;

    static EasyProtobufProgram()
    {
        // StopProgram is now built-in command
        stopProgram = new StopCommand();
        handlers.Add(stopProgram);
        var cmdlist = ConfigureCommands();
        handlers.AddRange(cmdlist);
        protobufWorker = new();
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
            new RsaCmd(),
            new XorCmd(),
        };
        return handlers;
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
            CommandHistory.PushExecuted(cmd);
            CommandHistoryLogger?.Info(cmd, "InputCommand");

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
                    prompt: $"Encountered error when handling command '{commandName}'. Please check your input.");
            }

        }
        await Task.Delay(10000);
    }
}
