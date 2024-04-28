using System.Text;
using XC.RSAUtil;
using YYHEggEgg.EasyProtobuf.Util;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal partial class RsaCmd
{
    private RsaKeyFeature? ParseKeyTypeStrings(IEnumerable<string> keyTypeStrings)
    {
        var outputKeyType = new RsaKeyFeature();
        bool? recordIsPrivate = null;
        foreach (var opt in keyTypeStrings)
        {
            if (opt == "Public" || opt == "Private")
            {
                if (recordIsPrivate != null)
                {
                    _logger.LogErro("Please specify Private or Public only once!");
                    return null;
                }
                recordIsPrivate = opt == "Private";
                continue;
            }

            if (Enum.TryParse<RsaKeyPadding>(opt, out var padding))
            {
                if (outputKeyType.Padding != RsaKeyPadding.Invalid)
                {
                    _logger.LogErro("Please specify key padding (Xml, Pkcs1, Pkcs8, Der) only once!");
                    return null;
                }
                outputKeyType.Padding = padding;
            }
            if (Enum.TryParse<RsaKeyFormat>(opt, out var format))
            {
                if (outputKeyType.Format != RsaKeyFormat.Invalid)
                {
                    _logger.LogErro("Please specify key format (Xml, Pem, Der) only once!");
                    return null;
                }
                outputKeyType.Format = format;
            }
        }

        if (outputKeyType.Format == RsaKeyFormat.Invalid && (outputKeyType.Padding == RsaKeyPadding.Pkcs1 || outputKeyType.Padding == RsaKeyPadding.Pkcs8))
            outputKeyType.Format = RsaKeyFormat.Pem;
        if (recordIsPrivate == null)
        {
            _logger.LogErro("Please specify whether to generate Public or Private key!");
            return null;
        }
        outputKeyType.IsPrivate = recordIsPrivate.Value;

        return outputKeyType;
    }

    public override async Task HandleAsync(RsaKeyConvertOption o)
    {
        byte[] keyBin = await o.GetKeyBytesAsync();

        var inputKeyType = RSAUtilBase.TreatRSAKeyType(keyBin);
        var outputKeyType = ParseKeyTypeStrings(o.OutputKeyType);
        if (outputKeyType == null) return;

        _logger.LogInfo($"Input key type: Format: {inputKeyType.Format}, Padding: {inputKeyType.Padding}, IsPrivate: {inputKeyType.IsPrivate}");
        _logger.LogInfo($"Output key type: Format: {outputKeyType.Format}, Padding: {outputKeyType.Padding}, IsPrivate: {outputKeyType.IsPrivate}");

        var res = RsaKeyConvert.Format(keyBin, inputKeyType, outputKeyType);
        if (o.SaveTo != null)
        {
            var savePath = Path.GetFullPath(o.SaveTo);
            File.WriteAllBytes(savePath, res);
            _logger.LogInfo($"Key saved to path: '{savePath}'.");
        }
        else
        {
            _logger.LogVerb(outputKeyType.Format.ToString());
            if (outputKeyType.Format == RsaKeyFormat.Der)
                _logger.LogWarn($"The output key is in a binary format; it may not be able to display correctly. You may need to specify '-s, --save' option and try again.");
            Tools.SetClipBoard(Encoding.UTF8.GetString(res));
            _logger.LogInfo($"Key output to clipboard.");
        }
    }
}
