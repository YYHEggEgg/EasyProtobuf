using Microsoft.Extensions.Logging;
using XC.RSAUtil;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal partial class RsaCmd
{
    public override async Task<bool> HandleAsync(RsaKeyGenOption o, CancellationToken cancellationToken)
    {
        var keyType = ParseKeyTypeStrings(o.OutputKeyType, true);
        if (keyType == null) return false;

        _logger.LogInformation("Generating key pair: Format: {}, Padding: {}, Size: {}", keyType.Format, keyType.Padding, o.KeySize);
        var newkeys = RsaKeyGenerator.GetKey(keyType, o.KeySize);
        var savePriPath = Path.GetFullPath(o.SavePrivateTo);
        await File.WriteAllBytesAsync(savePriPath, newkeys.PrivateKey);
        _logger.LogInformation("Private Key saved to path: '{savePriPath}'.", savePriPath);
        var savePubPath = Path.GetFullPath(o.SavePublicTo);
        await File.WriteAllBytesAsync(savePubPath, newkeys.PublicKey);
        _logger.LogInformation("Public Key saved to path: '{savePubPath}'.", savePubPath);
        return true;
    }
}
