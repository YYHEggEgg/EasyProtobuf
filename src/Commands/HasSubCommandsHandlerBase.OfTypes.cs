using CommandLine;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal abstract class HasSubCommandsHandlerBase<TCmdOption1> : HasSubCommandsHandlerBase
{
    public HasSubCommandsHandlerBase() : base([typeof(TCmdOption1)])
    {
    }

    public abstract Task HandleAsync(TCmdOption1 o);

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<TCmdOption1>(args)
            .MapResult(
                async (TCmdOption1 opt) => await HandleAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }
}

#region Powered By SlaveGPT™
internal abstract class HasSubCommandsHandlerBase<TCmdOption1, TCmdOption2> : HasSubCommandsHandlerBase
{
    public HasSubCommandsHandlerBase() : base([typeof(TCmdOption1), typeof(TCmdOption2)])
    {
    }

    public abstract Task HandleAsync(TCmdOption1 o);
    public abstract Task HandleAsync(TCmdOption2 o);

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<TCmdOption1, TCmdOption2>(args)
            .MapResult(
                async (TCmdOption1 opt) => await HandleAsync(opt),
                async (TCmdOption2 opt) => await HandleAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }
}

internal abstract class HasSubCommandsHandlerBase<TCmdOption1, TCmdOption2, TCmdOption3> : HasSubCommandsHandlerBase
{
    public HasSubCommandsHandlerBase() : base([typeof(TCmdOption1), typeof(TCmdOption2), typeof(TCmdOption3)])
    {
    }

    public abstract Task HandleAsync(TCmdOption1 o);
    public abstract Task HandleAsync(TCmdOption2 o);
    public abstract Task HandleAsync(TCmdOption3 o);

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<TCmdOption1, TCmdOption2, TCmdOption3>(args)
            .MapResult(
                async (TCmdOption1 opt) => await HandleAsync(opt),
                async (TCmdOption2 opt) => await HandleAsync(opt),
                async (TCmdOption3 opt) => await HandleAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }
}

internal abstract class HasSubCommandsHandlerBase<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4> : HasSubCommandsHandlerBase
{
    public HasSubCommandsHandlerBase() : base(
        [
            typeof(TCmdOption1),
            typeof(TCmdOption2),
            typeof(TCmdOption3),
            typeof(TCmdOption4)
        ])
    {
    }

    public abstract Task HandleAsync(TCmdOption1 o);
    public abstract Task HandleAsync(TCmdOption2 o);
    public abstract Task HandleAsync(TCmdOption3 o);
    public abstract Task HandleAsync(TCmdOption4 o);

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4>(args)
            .MapResult(
                async (TCmdOption1 opt) => await HandleAsync(opt),
                async (TCmdOption2 opt) => await HandleAsync(opt),
                async (TCmdOption3 opt) => await HandleAsync(opt),
                async (TCmdOption4 opt) => await HandleAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }
}

internal abstract class HasSubCommandsHandlerBase<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5> : HasSubCommandsHandlerBase
{
    public HasSubCommandsHandlerBase() : base(
        [
            typeof(TCmdOption1),
            typeof(TCmdOption2),
            typeof(TCmdOption3),
            typeof(TCmdOption4),
            typeof(TCmdOption5)
        ])
    {
    }

    public abstract Task HandleAsync(TCmdOption1 o);
    public abstract Task HandleAsync(TCmdOption2 o);
    public abstract Task HandleAsync(TCmdOption3 o);
    public abstract Task HandleAsync(TCmdOption4 o);
    public abstract Task HandleAsync(TCmdOption5 o);

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5>(args)
            .MapResult(
                async (TCmdOption1 opt) => await HandleAsync(opt),
                async (TCmdOption2 opt) => await HandleAsync(opt),
                async (TCmdOption3 opt) => await HandleAsync(opt),
                async (TCmdOption4 opt) => await HandleAsync(opt),
                async (TCmdOption5 opt) => await HandleAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }
}

internal abstract class HasSubCommandsHandlerBase<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6> : HasSubCommandsHandlerBase
{
    public HasSubCommandsHandlerBase() : base(
        [
            typeof(TCmdOption1),
            typeof(TCmdOption2),
            typeof(TCmdOption3),
            typeof(TCmdOption4),
            typeof(TCmdOption5),
            typeof(TCmdOption6)
        ])
    {
    }

    public abstract Task HandleAsync(TCmdOption1 o);
    public abstract Task HandleAsync(TCmdOption2 o);
    public abstract Task HandleAsync(TCmdOption3 o);
    public abstract Task HandleAsync(TCmdOption4 o);
    public abstract Task HandleAsync(TCmdOption5 o);
    public abstract Task HandleAsync(TCmdOption6 o);

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6>(args)
            .MapResult(
                async (TCmdOption1 opt) => await HandleAsync(opt),
                async (TCmdOption2 opt) => await HandleAsync(opt),
                async (TCmdOption3 opt) => await HandleAsync(opt),
                async (TCmdOption4 opt) => await HandleAsync(opt),
                async (TCmdOption5 opt) => await HandleAsync(opt),
                async (TCmdOption6 opt) => await HandleAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }
}

internal abstract class HasSubCommandsHandlerBase<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7> : HasSubCommandsHandlerBase
{
    public HasSubCommandsHandlerBase() : base(
        [
            typeof(TCmdOption1),
            typeof(TCmdOption2),
            typeof(TCmdOption3),
            typeof(TCmdOption4),
            typeof(TCmdOption5),
            typeof(TCmdOption6),
            typeof(TCmdOption7)
        ])
    {
    }

    public abstract Task HandleAsync(TCmdOption1 o);
    public abstract Task HandleAsync(TCmdOption2 o);
    public abstract Task HandleAsync(TCmdOption3 o);
    public abstract Task HandleAsync(TCmdOption4 o);
    public abstract Task HandleAsync(TCmdOption5 o);
    public abstract Task HandleAsync(TCmdOption6 o);
    public abstract Task HandleAsync(TCmdOption7 o);

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7>(args)
            .MapResult(
                async (TCmdOption1 opt) => await HandleAsync(opt),
                async (TCmdOption2 opt) => await HandleAsync(opt),
                async (TCmdOption3 opt) => await HandleAsync(opt),
                async (TCmdOption4 opt) => await HandleAsync(opt),
                async (TCmdOption5 opt) => await HandleAsync(opt),
                async (TCmdOption6 opt) => await HandleAsync(opt),
                async (TCmdOption7 opt) => await HandleAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }
}

internal abstract class HasSubCommandsHandlerBase<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8> : HasSubCommandsHandlerBase
{
    public HasSubCommandsHandlerBase() : base(
        [
            typeof(TCmdOption1),
            typeof(TCmdOption2),
            typeof(TCmdOption3),
            typeof(TCmdOption4),
            typeof(TCmdOption5),
            typeof(TCmdOption6),
            typeof(TCmdOption7),
            typeof(TCmdOption8)
        ])
    {
    }

    public abstract Task HandleAsync(TCmdOption1 o);
    public abstract Task HandleAsync(TCmdOption2 o);
    public abstract Task HandleAsync(TCmdOption3 o);
    public abstract Task HandleAsync(TCmdOption4 o);
    public abstract Task HandleAsync(TCmdOption5 o);
    public abstract Task HandleAsync(TCmdOption6 o);
    public abstract Task HandleAsync(TCmdOption7 o);
    public abstract Task HandleAsync(TCmdOption8 o);

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8>(args)
            .MapResult(
                async (TCmdOption1 opt) => await HandleAsync(opt),
                async (TCmdOption2 opt) => await HandleAsync(opt),
                async (TCmdOption3 opt) => await HandleAsync(opt),
                async (TCmdOption4 opt) => await HandleAsync(opt),
                async (TCmdOption5 opt) => await HandleAsync(opt),
                async (TCmdOption6 opt) => await HandleAsync(opt),
                async (TCmdOption7 opt) => await HandleAsync(opt),
                async (TCmdOption8 opt) => await HandleAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }
}

internal abstract class HasSubCommandsHandlerBase<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8, TCmdOption9> : HasSubCommandsHandlerBase
{
    public HasSubCommandsHandlerBase() : base(
        [
            typeof(TCmdOption1),
            typeof(TCmdOption2),
            typeof(TCmdOption3),
            typeof(TCmdOption4),
            typeof(TCmdOption5),
            typeof(TCmdOption6),
            typeof(TCmdOption7),
            typeof(TCmdOption8),
            typeof(TCmdOption9)
        ])
    {
    }

    public abstract Task HandleAsync(TCmdOption1 o);
    public abstract Task HandleAsync(TCmdOption2 o);
    public abstract Task HandleAsync(TCmdOption3 o);
    public abstract Task HandleAsync(TCmdOption4 o);
    public abstract Task HandleAsync(TCmdOption5 o);
    public abstract Task HandleAsync(TCmdOption6 o);
    public abstract Task HandleAsync(TCmdOption7 o);
    public abstract Task HandleAsync(TCmdOption8 o);
    public abstract Task HandleAsync(TCmdOption9 o);

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8, TCmdOption9>(args)
            .MapResult(
                async (TCmdOption1 opt) => await HandleAsync(opt),
                async (TCmdOption2 opt) => await HandleAsync(opt),
                async (TCmdOption3 opt) => await HandleAsync(opt),
                async (TCmdOption4 opt) => await HandleAsync(opt),
                async (TCmdOption5 opt) => await HandleAsync(opt),
                async (TCmdOption6 opt) => await HandleAsync(opt),
                async (TCmdOption7 opt) => await HandleAsync(opt),
                async (TCmdOption8 opt) => await HandleAsync(opt),
                async (TCmdOption9 opt) => await HandleAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }
}

internal abstract class HasSubCommandsHandlerBase<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8, TCmdOption9, TCmdOption10> : HasSubCommandsHandlerBase
{
    public HasSubCommandsHandlerBase() : base(
        [
            typeof(TCmdOption1),
            typeof(TCmdOption2),
            typeof(TCmdOption3),
            typeof(TCmdOption4),
            typeof(TCmdOption5),
            typeof(TCmdOption6),
            typeof(TCmdOption7),
            typeof(TCmdOption8),
            typeof(TCmdOption9),
            typeof(TCmdOption10)
        ])
    {
    }

    public abstract Task HandleAsync(TCmdOption1 o);
    public abstract Task HandleAsync(TCmdOption2 o);
    public abstract Task HandleAsync(TCmdOption3 o);
    public abstract Task HandleAsync(TCmdOption4 o);
    public abstract Task HandleAsync(TCmdOption5 o);
    public abstract Task HandleAsync(TCmdOption6 o);
    public abstract Task HandleAsync(TCmdOption7 o);
    public abstract Task HandleAsync(TCmdOption8 o);
    public abstract Task HandleAsync(TCmdOption9 o);
    public abstract Task HandleAsync(TCmdOption10 o);

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8, TCmdOption9, TCmdOption10>(args)
            .MapResult(
                async (TCmdOption1 opt) => await HandleAsync(opt),
                async (TCmdOption2 opt) => await HandleAsync(opt),
                async (TCmdOption3 opt) => await HandleAsync(opt),
                async (TCmdOption4 opt) => await HandleAsync(opt),
                async (TCmdOption5 opt) => await HandleAsync(opt),
                async (TCmdOption6 opt) => await HandleAsync(opt),
                async (TCmdOption7 opt) => await HandleAsync(opt),
                async (TCmdOption8 opt) => await HandleAsync(opt),
                async (TCmdOption9 opt) => await HandleAsync(opt),
                async (TCmdOption10 opt) => await HandleAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }
}

internal abstract class HasSubCommandsHandlerBase<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8, TCmdOption9, TCmdOption10, TCmdOption11> : HasSubCommandsHandlerBase
{
    public HasSubCommandsHandlerBase() : base(
        [
            typeof(TCmdOption1),
            typeof(TCmdOption2),
            typeof(TCmdOption3),
            typeof(TCmdOption4),
            typeof(TCmdOption5),
            typeof(TCmdOption6),
            typeof(TCmdOption7),
            typeof(TCmdOption8),
            typeof(TCmdOption9),
            typeof(TCmdOption10),
            typeof(TCmdOption11)
        ])
    {
    }

    public abstract Task HandleAsync(TCmdOption1 o);
    public abstract Task HandleAsync(TCmdOption2 o);
    public abstract Task HandleAsync(TCmdOption3 o);
    public abstract Task HandleAsync(TCmdOption4 o);
    public abstract Task HandleAsync(TCmdOption5 o);
    public abstract Task HandleAsync(TCmdOption6 o);
    public abstract Task HandleAsync(TCmdOption7 o);
    public abstract Task HandleAsync(TCmdOption8 o);
    public abstract Task HandleAsync(TCmdOption9 o);
    public abstract Task HandleAsync(TCmdOption10 o);
    public abstract Task HandleAsync(TCmdOption11 o);

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8, TCmdOption9, TCmdOption10, TCmdOption11>(args)
            .MapResult(
                async (TCmdOption1 opt) => await HandleAsync(opt),
                async (TCmdOption2 opt) => await HandleAsync(opt),
                async (TCmdOption3 opt) => await HandleAsync(opt),
                async (TCmdOption4 opt) => await HandleAsync(opt),
                async (TCmdOption5 opt) => await HandleAsync(opt),
                async (TCmdOption6 opt) => await HandleAsync(opt),
                async (TCmdOption7 opt) => await HandleAsync(opt),
                async (TCmdOption8 opt) => await HandleAsync(opt),
                async (TCmdOption9 opt) => await HandleAsync(opt),
                async (TCmdOption10 opt) => await HandleAsync(opt),
                async (TCmdOption11 opt) => await HandleAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }
}

internal abstract class HasSubCommandsHandlerBase<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8, TCmdOption9, TCmdOption10, TCmdOption11, TCmdOption12> : HasSubCommandsHandlerBase
{
    public HasSubCommandsHandlerBase() : base(
        [
            typeof(TCmdOption1),
            typeof(TCmdOption2),
            typeof(TCmdOption3),
            typeof(TCmdOption4),
            typeof(TCmdOption5),
            typeof(TCmdOption6),
            typeof(TCmdOption7),
            typeof(TCmdOption8),
            typeof(TCmdOption9),
            typeof(TCmdOption10),
            typeof(TCmdOption11),
            typeof(TCmdOption12)
        ])
    {
    }

    public abstract Task HandleAsync(TCmdOption1 o);
    public abstract Task HandleAsync(TCmdOption2 o);
    public abstract Task HandleAsync(TCmdOption3 o);
    public abstract Task HandleAsync(TCmdOption4 o);
    public abstract Task HandleAsync(TCmdOption5 o);
    public abstract Task HandleAsync(TCmdOption6 o);
    public abstract Task HandleAsync(TCmdOption7 o);
    public abstract Task HandleAsync(TCmdOption8 o);
    public abstract Task HandleAsync(TCmdOption9 o);
    public abstract Task HandleAsync(TCmdOption10 o);
    public abstract Task HandleAsync(TCmdOption11 o);
    public abstract Task HandleAsync(TCmdOption12 o);

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8, TCmdOption9, TCmdOption10, TCmdOption11, TCmdOption12>(args)
            .MapResult(
                async (TCmdOption1 opt) => await HandleAsync(opt),
                async (TCmdOption2 opt) => await HandleAsync(opt),
                async (TCmdOption3 opt) => await HandleAsync(opt),
                async (TCmdOption4 opt) => await HandleAsync(opt),
                async (TCmdOption5 opt) => await HandleAsync(opt),
                async (TCmdOption6 opt) => await HandleAsync(opt),
                async (TCmdOption7 opt) => await HandleAsync(opt),
                async (TCmdOption8 opt) => await HandleAsync(opt),
                async (TCmdOption9 opt) => await HandleAsync(opt),
                async (TCmdOption10 opt) => await HandleAsync(opt),
                async (TCmdOption11 opt) => await HandleAsync(opt),
                async (TCmdOption12 opt) => await HandleAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }
}

internal abstract class HasSubCommandsHandlerBase<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8, TCmdOption9, TCmdOption10, TCmdOption11, TCmdOption12, TCmdOption13> : HasSubCommandsHandlerBase
{
    public HasSubCommandsHandlerBase() : base(
        [
            typeof(TCmdOption1),
            typeof(TCmdOption2),
            typeof(TCmdOption3),
            typeof(TCmdOption4),
            typeof(TCmdOption5),
            typeof(TCmdOption6),
            typeof(TCmdOption7),
            typeof(TCmdOption8),
            typeof(TCmdOption9),
            typeof(TCmdOption10),
            typeof(TCmdOption11),
            typeof(TCmdOption12),
            typeof(TCmdOption13)
        ])
    {
    }

    public abstract Task HandleAsync(TCmdOption1 o);
    public abstract Task HandleAsync(TCmdOption2 o);
    public abstract Task HandleAsync(TCmdOption3 o);
    public abstract Task HandleAsync(TCmdOption4 o);
    public abstract Task HandleAsync(TCmdOption5 o);
    public abstract Task HandleAsync(TCmdOption6 o);
    public abstract Task HandleAsync(TCmdOption7 o);
    public abstract Task HandleAsync(TCmdOption8 o);
    public abstract Task HandleAsync(TCmdOption9 o);
    public abstract Task HandleAsync(TCmdOption10 o);
    public abstract Task HandleAsync(TCmdOption11 o);
    public abstract Task HandleAsync(TCmdOption12 o);
    public abstract Task HandleAsync(TCmdOption13 o);

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8, TCmdOption9, TCmdOption10, TCmdOption11, TCmdOption12, TCmdOption13>(args)
            .MapResult(
                async (TCmdOption1 opt) => await HandleAsync(opt),
                async (TCmdOption2 opt) => await HandleAsync(opt),
                async (TCmdOption3 opt) => await HandleAsync(opt),
                async (TCmdOption4 opt) => await HandleAsync(opt),
                async (TCmdOption5 opt) => await HandleAsync(opt),
                async (TCmdOption6 opt) => await HandleAsync(opt),
                async (TCmdOption7 opt) => await HandleAsync(opt),
                async (TCmdOption8 opt) => await HandleAsync(opt),
                async (TCmdOption9 opt) => await HandleAsync(opt),
                async (TCmdOption10 opt) => await HandleAsync(opt),
                async (TCmdOption11 opt) => await HandleAsync(opt),
                async (TCmdOption12 opt) => await HandleAsync(opt),
                async (TCmdOption13 opt) => await HandleAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }
}

internal abstract class HasSubCommandsHandlerBase<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8, TCmdOption9, TCmdOption10, TCmdOption11, TCmdOption12, TCmdOption13, TCmdOption14> : HasSubCommandsHandlerBase
{
    public HasSubCommandsHandlerBase() : base(
        [
            typeof(TCmdOption1),
            typeof(TCmdOption2),
            typeof(TCmdOption3),
            typeof(TCmdOption4),
            typeof(TCmdOption5),
            typeof(TCmdOption6),
            typeof(TCmdOption7),
            typeof(TCmdOption8),
            typeof(TCmdOption9),
            typeof(TCmdOption10),
            typeof(TCmdOption11),
            typeof(TCmdOption12),
            typeof(TCmdOption13),
            typeof(TCmdOption14)
        ])
    {
    }

    public abstract Task HandleAsync(TCmdOption1 o);
    public abstract Task HandleAsync(TCmdOption2 o);
    public abstract Task HandleAsync(TCmdOption3 o);
    public abstract Task HandleAsync(TCmdOption4 o);
    public abstract Task HandleAsync(TCmdOption5 o);
    public abstract Task HandleAsync(TCmdOption6 o);
    public abstract Task HandleAsync(TCmdOption7 o);
    public abstract Task HandleAsync(TCmdOption8 o);
    public abstract Task HandleAsync(TCmdOption9 o);
    public abstract Task HandleAsync(TCmdOption10 o);
    public abstract Task HandleAsync(TCmdOption11 o);
    public abstract Task HandleAsync(TCmdOption12 o);
    public abstract Task HandleAsync(TCmdOption13 o);
    public abstract Task HandleAsync(TCmdOption14 o);

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8, TCmdOption9, TCmdOption10, TCmdOption11, TCmdOption12, TCmdOption13, TCmdOption14>(args)
            .MapResult(
                async (TCmdOption1 opt) => await HandleAsync(opt),
                async (TCmdOption2 opt) => await HandleAsync(opt),
                async (TCmdOption3 opt) => await HandleAsync(opt),
                async (TCmdOption4 opt) => await HandleAsync(opt),
                async (TCmdOption5 opt) => await HandleAsync(opt),
                async (TCmdOption6 opt) => await HandleAsync(opt),
                async (TCmdOption7 opt) => await HandleAsync(opt),
                async (TCmdOption8 opt) => await HandleAsync(opt),
                async (TCmdOption9 opt) => await HandleAsync(opt),
                async (TCmdOption10 opt) => await HandleAsync(opt),
                async (TCmdOption11 opt) => await HandleAsync(opt),
                async (TCmdOption12 opt) => await HandleAsync(opt),
                async (TCmdOption13 opt) => await HandleAsync(opt),
                async (TCmdOption14 opt) => await HandleAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }
}

internal abstract class HasSubCommandsHandlerBase<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8, TCmdOption9, TCmdOption10, TCmdOption11, TCmdOption12, TCmdOption13, TCmdOption14, TCmdOption15> : HasSubCommandsHandlerBase
{
    public HasSubCommandsHandlerBase() : base(
        [
            typeof(TCmdOption1),
            typeof(TCmdOption2),
            typeof(TCmdOption3),
            typeof(TCmdOption4),
            typeof(TCmdOption5),
            typeof(TCmdOption6),
            typeof(TCmdOption7),
            typeof(TCmdOption8),
            typeof(TCmdOption9),
            typeof(TCmdOption10),
            typeof(TCmdOption11),
            typeof(TCmdOption12),
            typeof(TCmdOption13),
            typeof(TCmdOption14),
            typeof(TCmdOption15)
        ])
    {
    }

    public abstract Task HandleAsync(TCmdOption1 o);
    public abstract Task HandleAsync(TCmdOption2 o);
    public abstract Task HandleAsync(TCmdOption3 o);
    public abstract Task HandleAsync(TCmdOption4 o);
    public abstract Task HandleAsync(TCmdOption5 o);
    public abstract Task HandleAsync(TCmdOption6 o);
    public abstract Task HandleAsync(TCmdOption7 o);
    public abstract Task HandleAsync(TCmdOption8 o);
    public abstract Task HandleAsync(TCmdOption9 o);
    public abstract Task HandleAsync(TCmdOption10 o);
    public abstract Task HandleAsync(TCmdOption11 o);
    public abstract Task HandleAsync(TCmdOption12 o);
    public abstract Task HandleAsync(TCmdOption13 o);
    public abstract Task HandleAsync(TCmdOption14 o);
    public abstract Task HandleAsync(TCmdOption15 o);

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8, TCmdOption9, TCmdOption10, TCmdOption11, TCmdOption12, TCmdOption13, TCmdOption14, TCmdOption15>(args)
            .MapResult(
                async (TCmdOption1 opt) => await HandleAsync(opt),
                async (TCmdOption2 opt) => await HandleAsync(opt),
                async (TCmdOption3 opt) => await HandleAsync(opt),
                async (TCmdOption4 opt) => await HandleAsync(opt),
                async (TCmdOption5 opt) => await HandleAsync(opt),
                async (TCmdOption6 opt) => await HandleAsync(opt),
                async (TCmdOption7 opt) => await HandleAsync(opt),
                async (TCmdOption8 opt) => await HandleAsync(opt),
                async (TCmdOption9 opt) => await HandleAsync(opt),
                async (TCmdOption10 opt) => await HandleAsync(opt),
                async (TCmdOption11 opt) => await HandleAsync(opt),
                async (TCmdOption12 opt) => await HandleAsync(opt),
                async (TCmdOption13 opt) => await HandleAsync(opt),
                async (TCmdOption14 opt) => await HandleAsync(opt),
                async (TCmdOption15 opt) => await HandleAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }
}

internal abstract class HasSubCommandsHandlerBase<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8, TCmdOption9, TCmdOption10, TCmdOption11, TCmdOption12, TCmdOption13, TCmdOption14, TCmdOption15, TCmdOption16> : HasSubCommandsHandlerBase
{
    public HasSubCommandsHandlerBase() : base(
        [
            typeof(TCmdOption1),
            typeof(TCmdOption2),
            typeof(TCmdOption3),
            typeof(TCmdOption4),
            typeof(TCmdOption5),
            typeof(TCmdOption6),
            typeof(TCmdOption7),
            typeof(TCmdOption8),
            typeof(TCmdOption9),
            typeof(TCmdOption10),
            typeof(TCmdOption11),
            typeof(TCmdOption12),
            typeof(TCmdOption13),
            typeof(TCmdOption14),
            typeof(TCmdOption15),
            typeof(TCmdOption16)
        ])
    {
    }

    public abstract Task HandleAsync(TCmdOption1 o);
    public abstract Task HandleAsync(TCmdOption2 o);
    public abstract Task HandleAsync(TCmdOption3 o);
    public abstract Task HandleAsync(TCmdOption4 o);
    public abstract Task HandleAsync(TCmdOption5 o);
    public abstract Task HandleAsync(TCmdOption6 o);
    public abstract Task HandleAsync(TCmdOption7 o);
    public abstract Task HandleAsync(TCmdOption8 o);
    public abstract Task HandleAsync(TCmdOption9 o);
    public abstract Task HandleAsync(TCmdOption10 o);
    public abstract Task HandleAsync(TCmdOption11 o);
    public abstract Task HandleAsync(TCmdOption12 o);
    public abstract Task HandleAsync(TCmdOption13 o);
    public abstract Task HandleAsync(TCmdOption14 o);
    public abstract Task HandleAsync(TCmdOption15 o);
    public abstract Task HandleAsync(TCmdOption16 o);

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<TCmdOption1, TCmdOption2, TCmdOption3, TCmdOption4, TCmdOption5, TCmdOption6, TCmdOption7, TCmdOption8, TCmdOption9, TCmdOption10, TCmdOption11, TCmdOption12, TCmdOption13, TCmdOption14, TCmdOption15, TCmdOption16>(args)
            .MapResult(
                async (TCmdOption1 opt) => await HandleAsync(opt),
                async (TCmdOption2 opt) => await HandleAsync(opt),
                async (TCmdOption3 opt) => await HandleAsync(opt),
                async (TCmdOption4 opt) => await HandleAsync(opt),
                async (TCmdOption5 opt) => await HandleAsync(opt),
                async (TCmdOption6 opt) => await HandleAsync(opt),
                async (TCmdOption7 opt) => await HandleAsync(opt),
                async (TCmdOption8 opt) => await HandleAsync(opt),
                async (TCmdOption9 opt) => await HandleAsync(opt),
                async (TCmdOption10 opt) => await HandleAsync(opt),
                async (TCmdOption11 opt) => await HandleAsync(opt),
                async (TCmdOption12 opt) => await HandleAsync(opt),
                async (TCmdOption13 opt) => await HandleAsync(opt),
                async (TCmdOption14 opt) => await HandleAsync(opt),
                async (TCmdOption15 opt) => await HandleAsync(opt),
                async (TCmdOption16 opt) => await HandleAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }
}
#endregion

#region SlaveGPT™'s ending words
// 在C#中，泛型类最多只能有16个类型参数。这是C#语言规范所规定的一个限制。因此，创建一个具有17个泛型参数的类是不符合C#规范的。
//
// 如果你需要处理更多的类型，你可以考虑使用其他方法来实现，例如使用一个泛型基类和多个继承它的非泛型子类，或者使用泛型接口和实现这些接口的非泛型类。另外，也可以考虑使用元组（Tuple）或者字典（Dictionary）来传递和处理更多的类型。
//
// 如果你确实需要支持类似17个不同命令选项这样的场景，你可能需要重新设计你的命令行解析器的架构，以便它不依赖于泛型参数的数量。这可能包括使用反射、动态类型或者构建一个基于策略模式的灵活系统。
#endregion
