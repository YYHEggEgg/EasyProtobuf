using XC.RSAUtil;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal partial class RsaCmd
{
    public override async Task HandleAsync(RsaKeyGenOption o)
    {
        var keyType = ParseKeyTypeStrings(o.OutputKeyType);
        if (keyType == null) return;

        _logger.LogInfo($"Generating key pair: Format: {keyType.Format}, Padding: {keyType.Padding}, Size: {o.KeySize}");
        var newkeys = RsaKeyGenerator.GetKey(keyType, o.KeySize);
        var savePriPath = Path.GetFullPath(o.SavePrivateTo);
        await File.WriteAllBytesAsync(savePriPath, newkeys.PrivateKey);
        _logger.LogInfo($"Private Key saved to path: '{savePriPath}'.");
        var savePubPath = Path.GetFullPath(o.SavePublicTo);
        await File.WriteAllBytesAsync(savePubPath, newkeys.PublicKey);
        _logger.LogInfo($"Public Key saved to path: '{savePubPath}'.");
    }
}
