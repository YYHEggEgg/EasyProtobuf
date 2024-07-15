using Microsoft.Extensions.Logging;
using YYHEggEgg.EasyProtobuf.Util;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal partial class RsaCmd
{
    public override async Task HandleAsync(RsaSignOption o)
    {
        var rsa = o.GetRSAWorker();
        var rawdata = o.Data;
        var signature = rsa.SignData(rawdata, o.HashAlgorithm, o.Padding);
        _logger.LogInformation("Created signature for input {len} bytes.", rawdata.Length);
        await Tools.SetClipBoardAsync(Convert.ToBase64String(signature));
    }
}
