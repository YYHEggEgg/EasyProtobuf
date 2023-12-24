namespace YYHEggEgg.EasyProtobuf.Commands;

internal partial class RsaCmd
{
    private Task HandleVerifyAsync(RsaVerifyOption o)
    {
        var rsa = o.GetRSAWorker();
        var rawdata = o.Data;
        var verificationOK = rsa.VerifyData(rawdata, o.Signature, o.HashAlgorithm, o.Padding);
        if (verificationOK) _logger.LogInfo($"Verification OK");
        else _logger.LogWarn($"Verification Failed");
        return Task.CompletedTask;
    }
}
