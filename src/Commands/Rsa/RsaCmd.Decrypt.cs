using YYHEggEgg.EasyProtobuf.Util;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal partial class RsaCmd
{
    private async Task HandleDecryptAsync(RsaDecryptOption o)
    {
        var rsa = o.GetRSAWorker();
        var encrypted = o.Data;
        var decrypted = rsa.RsaDecrypt(encrypted, o.Padding);
        _logger.LogInfo($"Decrypted {encrypted.Length} bytes -> {decrypted.Length} bytes.");
        await Tools.SetClipBoardAsync(Convert.ToBase64String(decrypted));
    }
}
