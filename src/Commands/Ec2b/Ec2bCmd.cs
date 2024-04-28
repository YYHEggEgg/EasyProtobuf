using AssetLib.Formats;
using CommandLine;
using YYHEggEgg.EasyProtobuf.Util;

namespace YYHEggEgg.EasyProtobuf.Commands;

[Verb("get_key", true)]
internal class Ec2bGetKeyOption
{
    [Value(0, Required = true)]
    public IEnumerable<string>? Data { get; set; }
}

[Verb("encrypt", false)]
internal class Ec2bEncryptOption
{
    [Value(0, Required = true)]
    public ulong KeySeed { get; set; }
}

internal class Ec2bCmd : HasSubCommandsHandlerBase<Ec2bGetKeyOption, Ec2bEncryptOption>
{
    public override string CommandName => "ec2b";

    public override string Description => "Make operations on dispatch secret_key/secret_seed.";

    public override string Usage => $"ec2b get_key <content_bindata(base64/hex)>{Environment.NewLine}" +
        EasyInput.MultipleInputNotice +
        $"{Environment.NewLine}" +
        $"{Environment.NewLine}" +
        $"Notice: <color=Yellow>If you're using Windows Terminal, press Ctrl+Alt+V to paste data with multiple lines.</color>{Environment.NewLine}" +
        $"{Environment.NewLine}" +
        $"ec2b encrypt <uint64_t_seed>";

    public override void CleanUp()
    {
        throw new NotImplementedException();
    }

    public override async Task HandleAsync(Ec2bGetKeyOption o)
    {
        var read = EasyInput.TryPreProcess(o.Data ?? [], 1);
        if (read.InputType != EasyInputType.Base64
            && read.InputType != EasyInputType.Hex)
        {
            _logger.LogErro($"The input type isn't supported!");
            return;
        }
        var hexkey = Convert.ToHexString(Ec2b.Decrypt(read.ToByteArray()));
        _logger.LogInfo(hexkey);
        await Tools.SetClipBoardAsync(hexkey);
    }

    public override async Task HandleAsync(Ec2bEncryptOption o)
    {
        var hexseed = Convert.ToHexString(Ec2b.Encrypt(o.KeySeed));
        _logger.LogInfo(hexseed);
        await Tools.SetClipBoardAsync(hexseed);
    }
}
