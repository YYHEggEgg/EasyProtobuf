using Microsoft.Extensions.Logging;
using YYHEggEgg.EasyProtobuf.Util;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal partial class RsaCmd
{
    public override async Task<bool> HandleAsync(RsaDecryptOption o, CancellationToken cancellationToken)
    {
        var rsa = o.GetRSAWorker();
        var encrypted = o.Data;
        var decrypted = rsa.RsaDecrypt(encrypted, o.Padding);
        _logger.LogInformation("Decrypted {len} bytes -> {len} bytes.", encrypted.Length, decrypted.Length);
        await Tools.SetClipBoardAsync(Convert.ToBase64String(decrypted));
        return true;
    }
}
