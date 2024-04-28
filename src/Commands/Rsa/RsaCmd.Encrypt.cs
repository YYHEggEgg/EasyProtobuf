using YYHEggEgg.EasyProtobuf.Util;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal partial class RsaCmd
{
    public override async Task HandleAsync(RsaEncryptOption o)
    {
        var rsa = o.GetRSAWorker();
        var rawdata = o.Data;
        var encrypted = rsa.RsaEncrypt(rawdata, o.Padding);
        _logger.LogInfo($"Encrypted {rawdata.Length} bytes -> {encrypted.Length} bytes.");
        await Tools.SetClipBoardAsync(Convert.ToBase64String(encrypted));
    }
}
