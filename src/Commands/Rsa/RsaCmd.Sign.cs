using YYHEggEgg.EasyProtobuf.Util;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal partial class RsaCmd
{
    private async Task HandleSignAsync(RsaSignOption o)
    {
        var rsa = o.GetRSAWorker();
        var rawdata = o.Data;
        var signature = rsa.SignData(rawdata, o.HashAlgorithm, o.Padding);
        _logger.LogInfo($"Created signature for input {rawdata.Length} bytes.");
        await Tools.SetClipBoardAsync(Convert.ToBase64String(signature));
    }
}
