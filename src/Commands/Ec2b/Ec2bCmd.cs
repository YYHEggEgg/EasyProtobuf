using AssetLib.Formats;
using CommandLine;
using Microsoft.Extensions.Logging;
using YYHEggEgg.EasyProtobuf.Util;
using YYHEggEgg.Shell;

namespace YYHEggEgg.EasyProtobuf.Commands;

[Verb("get_key", true)]
internal class Ec2bGetKeyOption
{
    [Value(0, Required = true, MetaName = "content_bindata(base64/hex)")]
    public IEnumerable<string>? Data { get; set; }
}

[Verb("encrypt", false)]
internal class Ec2bEncryptOption
{
    [Value(0, Required = true, MetaName = "uint64_t_seed")]
    public ulong KeySeed { get; set; }
}

internal class Ec2bCmd : HasSubCommandsHandlerBase<Ec2bGetKeyOption, Ec2bEncryptOption>
{
    public override string CommandName => "ec2b";

    public override string Description => "Make operations on dispatch secret_key/secret_seed.";

    public override void CleanUp()
    {
        throw new NotImplementedException();
    }

    protected override Dictionary<string, IEnumerable<string>>? SubcommandAdditionalDescLinesMap => new()
    {
        ["get_key"] =
            [
                EasyInput.MultipleInputNotice,
                string.Empty,
                "Notice: <color=Yellow>If you're using Windows Terminal, press Ctrl+Alt+V to paste data with multiple lines.</color>",
            ],
    };

    public override async Task<bool> HandleAsync(Ec2bGetKeyOption o, CancellationToken cancellationToken)
    {
        var read = EasyInput.TryPreProcess(o.Data ?? []);
        if (read.InputType != EasyInputType.Base64
            && read.InputType != EasyInputType.Hex)
        {
            _logger.LogError("The input type {type} isn't supported!", read.InputType);
            return false;
        }
        var hexkey = Convert.ToHexString(Ec2b.Decrypt(read.ToByteArray()));
        _logger.LogInformation("{bin}", hexkey);
        await Tools.SetClipBoardAsync(hexkey);
        return true;
    }

    public override async Task<bool> HandleAsync(Ec2bEncryptOption o, CancellationToken cancellationToken)
    {
        var hexseed = Convert.ToHexString(Ec2b.Encrypt(o.KeySeed));
        _logger.LogInformation("{bin}", hexseed);
        await Tools.SetClipBoardAsync(hexseed);
        return true;
    }
}
