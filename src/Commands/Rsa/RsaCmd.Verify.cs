using Microsoft.Extensions.Logging;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal partial class RsaCmd
{
    public override Task HandleAsync(RsaVerifyOption o)
    {
        var rsa = o.GetRSAWorker();
        var rawdata = o.Data;
        var verificationOK = rsa.VerifyData(rawdata, o.Signature, o.HashAlgorithm, o.Padding);
        if (verificationOK) _logger.LogInformation($"Verification OK");
        else _logger.LogWarning($"Verification Failed");
        return Task.CompletedTask;
    }
}
