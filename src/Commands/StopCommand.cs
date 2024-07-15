using YYHEggEgg.Shell;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal class StopCommand : CommandHandlerBase
{
    public override string CommandName => "stop";

    public override string Description => "Close the program.";

    public override IEnumerable<string> UsageLines => ["stop"];

    public override Task HandleAsync(string argList)
    {
        Environment.Exit(0);
        return Task.CompletedTask;
    }
}
