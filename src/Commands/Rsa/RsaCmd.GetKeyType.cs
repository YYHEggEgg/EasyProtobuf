using XC.RSAUtil;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal partial class RsaCmd
{
    private async Task HandleGetKeyTypeAsync(RsaGetKeyTypeOption o)
    {
        var keyBin = await o.GetKeyBytesAsync();
        var keyType = RSAUtilBase.TreatRSAKeyType(keyBin);
        var keyActual = RSAUtilBase.LoadRSAKey(keyBin);
        _logger.LogInfo($"The input key is of:");
        _logger.LogInfo($"- Size: {keyActual.PublicRsa?.KeySize}-bit");
        _logger.LogInfo($"- Format: {keyType.Format}");
        _logger.LogInfo($"- Padding: {keyType.Padding}");
        _logger.LogInfo($"- Private/Public: {(keyActual.PrivateRsa == null ? "Public" : "Private")}");
    }
}
