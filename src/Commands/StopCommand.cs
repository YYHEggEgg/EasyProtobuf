
using YYHEggEgg.EasyProtobuf.Configuration;
using YYHEggEgg.Logger;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal class StopCommand : CommandHandlerBase
{
    public override string CommandName => "stop";

    public override string Description => "Close the program.";

    public override string Usage => "stop";

    public override Task HandleAsync(string argList)
    {
        Log.Info($"Thanks for using EasyProtobuf!");
        // Config.FlushTo("config.json");
        Environment.Exit(0);
        return Task.CompletedTask;
    }
}
