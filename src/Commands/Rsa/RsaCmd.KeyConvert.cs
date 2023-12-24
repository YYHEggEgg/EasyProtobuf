using TextCopy;
using XC.RSAUtil;
using YYHEggEgg.EasyProtobuf.Util;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal partial class RsaCmd
{
    private async Task HandleKeyConvertAsync(RsaKeyConvertOption o)
    {
        string? keystr;
        if (o.InputKeyFilePath != null) keystr = File.ReadAllText(o.InputKeyFilePath);
        else if (o.ClipboardInput) 
        {
            keystr = await ClipboardService.GetTextAsync();
            if (keystr == null)
            {
                _logger.LogErro($"Can't get the key from clipboard!");
                return;
            }
        }
        else
        {
            _logger.LogErro($"Please specify either the input file path or '--cb-in' option!");
            return;
        }

        var inputKeyType = RSAUtilBase.TreatRSAKeyType(keystr);
        var outputKeyType = RsaKeyType.None;
        foreach (var opt in o.OutputKeyType) outputKeyType |= Enum.Parse<RsaKeyType>(opt);
        await Tools.SetClipBoardAsync(RsaKeyConvert.Format(keystr, inputKeyType, outputKeyType));
        _logger.LogInfo($"Key output to clipboard.");
    }
}
