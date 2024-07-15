using Microsoft.Extensions.Logging;
using YYHEggEgg.EasyProtobuf.Util;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal partial class RsaCmd
{
    public override async Task HandleAsync(RsaEncryptOption o)
    {
        var rsa = o.GetRSAWorker();
        var rawdata = o.Data;
        var encrypted = rsa.RsaEncrypt(rawdata, o.Padding);
        _logger.LogInformation("Encrypted {len} bytes -> {len} bytes.", rawdata.Length, encrypted.Length);
        await Tools.SetClipBoardAsync(Convert.ToBase64String(encrypted));
    }
}
