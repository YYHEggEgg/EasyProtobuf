using CommandLine;
using TextCopy;
using XC.RSAUtil;
using YYHEggEgg.EasyProtobuf.Util;

namespace YYHEggEgg.EasyProtobuf.Commands;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
internal class RsaKeyConvertOption
{
    [Value(0, Required = false, HelpText = "The path of input key file.")]
    public string InputKeyFilePath { get; set; }
    [Option("cb-in", Required = false, Default = false, HelpText = "Get input from the clipboard.")]
    public bool ClipboardInput { get; set; }
    [Option('o', "outkey", Required = true, HelpText = "The output key type you demand.")]
    public IEnumerable<string> OutputKeyType { get; set; }
}
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

internal class RsaKeyConvertCmd : StandardCommandHandler<RsaKeyConvertOption>
{
    public override string CommandName => "rsakeyconv";

    public override string Description => "Convert the provided RSA key (PEM or XML) into any supported format.";

    public override string Usage => $"rsakeyconv {Environment.NewLine}" +
        $"  <input-key-filePath>  {Environment.NewLine}" +
        $"  (or --cb-in: Get input from the clipboard.) {Environment.NewLine}" +
        $"  -o, --outkey [RSA Key Format Identifiers] {Environment.NewLine}" +
        $" {Environment.NewLine}" +
        $"  Avaliable Format identifier: 'Public', 'Private', 'Xml', 'Pkcs1', 'Pkcs8' {Environment.NewLine}" +
        $"  Example: rsakeyconv key.pem -o Public Xml";

    public override async Task HandleAsync(RsaKeyConvertOption o)
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