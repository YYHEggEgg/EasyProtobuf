using Microsoft.Extensions.Logging;
using System.Text;
using XC.RSAUtil;
using YYHEggEgg.EasyProtobuf.Util;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal partial class RsaCmd
{
    private RsaKeyFeature? ParseKeyTypeStrings(IEnumerable<string> keyTypeStrings, bool optionalIsPrivate = false)
    {
        var outputKeyType = new RsaKeyFeature();
        bool? recordIsPrivate = null;
        foreach (var opt in keyTypeStrings)
        {
            if (opt == "Public" || opt == "Private")
            {
                if (recordIsPrivate != null)
                {
                    _logger.LogError("Please specify Private or Public only once!");
                    return null;
                }
                recordIsPrivate = opt == "Private";
                continue;
            }

            if (Enum.TryParse<RsaKeyPadding>(opt, out var padding))
            {
                if (outputKeyType.Padding != RsaKeyPadding.Invalid)
                {
                    _logger.LogError("Please specify key padding (Xml, Pkcs1, Pkcs8, Der) only once!");
                    return null;
                }
                outputKeyType.Padding = padding;
            }
            if (Enum.TryParse<RsaKeyFormat>(opt, out var format))
            {
                if (outputKeyType.Format != RsaKeyFormat.Invalid)
                {
                    _logger.LogError("Please specify key format (Xml, Pem, Der) only once!");
                    return null;
                }
                outputKeyType.Format = format;
            }
        }

        if (outputKeyType.Format == RsaKeyFormat.Invalid && (outputKeyType.Padding == RsaKeyPadding.Pkcs1 || outputKeyType.Padding == RsaKeyPadding.Pkcs8))
            outputKeyType.Format = RsaKeyFormat.Pem;
        if (recordIsPrivate == null)
        {
            if (optionalIsPrivate)
                outputKeyType.IsPrivate = false;
            else
            {
                _logger.LogError("Please specify whether to generate Public or Private key!");
                return null;
            }
        }
        else outputKeyType.IsPrivate = recordIsPrivate.Value;

        return outputKeyType;
    }

    public override async Task<bool> HandleAsync(RsaKeyConvertOption o, CancellationToken cancellationToken)
    {
        byte[] keyBin = await o.GetKeyBytesAsync();

        var inputKeyType = RSAUtilBase.TreatRSAKeyType(keyBin);
        var outputKeyType = ParseKeyTypeStrings(o.OutputKeyType);
        if (outputKeyType == null) return false;

        _logger.LogInformation("Input key type: Format: {}, Padding: {}, IsPrivate: {}", inputKeyType.Format, inputKeyType.Padding, inputKeyType.IsPrivate);
        _logger.LogInformation("Output key type: Format: {}, Padding: {}, IsPrivate: {}", outputKeyType.Format, outputKeyType.Padding, outputKeyType.IsPrivate);

        var res = RsaKeyConvert.Format(keyBin, outputKeyType);
        if (o.SaveTo != null)
        {
            var savePath = Path.GetFullPath(o.SaveTo);
            File.WriteAllBytes(savePath, res);
            _logger.LogInformation("Key saved to path: '{path}'.", savePath);
        }
        else
        {
            _logger.LogTrace("{format}", outputKeyType.Format.ToString());
            if (outputKeyType.Format == RsaKeyFormat.Der)
                _logger.LogWarning("The output key is in a binary format; " +
                    "it may not be able to display correctly. " +
                    "You may need to specify '-s, --save' option and try again.");
            Tools.SetClipBoard(Encoding.UTF8.GetString(res));
            _logger.LogInformation("Key output to clipboard.");
        }
        return true;
    }
}
