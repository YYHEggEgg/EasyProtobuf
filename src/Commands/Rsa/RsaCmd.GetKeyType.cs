using Microsoft.Extensions.Logging;
using XC.RSAUtil;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal partial class RsaCmd
{
    public override async Task HandleAsync(RsaGetKeyTypeOption o)
    {
        var keyBin = await o.GetKeyBytesAsync();
        var keyType = RSAUtilBase.TreatRSAKeyType(keyBin);
        var keyActual = RSAUtilBase.LoadRSAKey(keyBin);
        _logger.LogInformation("The input key is of:");
        _logger.LogInformation("- Size: {num}-bit", keyActual.PublicRsa?.KeySize);
        _logger.LogInformation("- Format: {format}", keyType.Format);
        _logger.LogInformation("- Padding: {padding}", keyType.Padding);
        _logger.LogInformation("- Private/Public: {desc}", keyActual.PrivateRsa == null ? "Public" : "Private");
    }
}
