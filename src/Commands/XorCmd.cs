using CommandLine;
using YYHEggEgg.EasyProtobuf.Util;

namespace YYHEggEgg.EasyProtobuf.Commands;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
[Verb("set-key", false, HelpText = "Set the default key for XOR operations.")]
internal class XorSetDefaultKeyOption
{
    [Value(0, Required = true, HelpText = "The demanded default key for XOR command.")]
    public IEnumerable<string> Key { get; set; }
}

[Verb("operate", true, HelpText = "Do a XOR operation.")]
internal class XorOperateOption
{
    [Option('k', "xorkey", Required = false, Default = null, HelpText = "Set the default key for XOR command.")]
    public IEnumerable<string>? Key { get; set; }
    [Value(0, Required = true, HelpText = "The value to be XOR decrypted.")]
    public IEnumerable<string> Value { get; set; }
    [Option("validate-startswith", Required = false, Default = null, HelpText = "Validate the result starts with a certain pattern.")]
    public IEnumerable<string>? ValidateStartsWith { get; set; }
    [Option("validate-endswith", Required = false, Default = null, HelpText = "Validate the result ends with a certain pattern.")]
    public IEnumerable<string>? ValidateEndsWith { get; set; }
}
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

internal class XorCmd : CommandHandlerBase
{
    public override string CommandName => "xor";

    public override string Description => "Perform the XOR decryption and analyze the result.";

    public override string Usage => $"xor [command] <args> {Environment.NewLine}" +
        $"  command 'operate' (default): Do a XOR operation. {Environment.NewLine}" +
        $"    xor <value>                The HEX / Base64 Content that should be decrypted. {Environment.NewLine}" +
        $"        -k, --xorkey           Set the default key for XOR command. {Environment.NewLine}" +
        $"        --validate-startswith  Validate the result starts with a certain pattern. {Environment.NewLine}" +
        $"        --validate-endswith    Validate the result ends with a certain pattern. {Environment.NewLine}" +
        $" {Environment.NewLine}" +
        $"  command 'set-key': Set the default key for XOR operations. {Environment.NewLine}" +
        $"    xor set-key {Environment.NewLine}" +
        $"        <xorkey>               The demanded default key for XOR command. {Environment.NewLine}" +
        $" {Environment.NewLine}" +
        EasyInput.MultipleInputNotice +
        $" {Environment.NewLine}" +
        $" {Environment.NewLine}" +
        $"Notice: <color=Yellow>If you're using Windows Terminal, press Ctrl+Alt+V to paste data with multiple lines.</color>";

    public override async Task HandleAsync(string argList)
    {
        var parseres = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<XorOperateOption, XorSetDefaultKeyOption>(parseres)
            .MapResult(
                async (XorOperateOption opt) => await HandleOperateAsync(opt),
                async (XorSetDefaultKeyOption opt) => await HandleSetDefaultKeyAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                }
            );
    }

    private byte[]? default_key = null;

    private Task HandleSetDefaultKeyAsync(XorSetDefaultKeyOption opt)
    {
        default_key = EasyInput.TryPreProcess(opt.Key).ToByteArray();
        _logger.LogInfo($"Successfully set default key: {default_key.Length} bytes.");
        return Task.CompletedTask;
    }

    private async Task HandleOperateAsync(XorOperateOption opt)
    {
        byte[]? key = default_key;
        if (opt.Key != null && opt.Key.Any()) key = EasyInput.TryPreProcess(opt.Key).ToByteArray();
        if (key == null)
        {
            _logger.LogErro($"Please give the using XOR key by '-k' option, or set the default key with 'xor set-key' command.");
            return;
        }

        var value = EasyInput.TryPreProcess(opt.Value).ToByteArray();
        XorDecrypt(value, key);
        await Tools.SetClipBoardAsync(Convert.ToHexString(value));
        _logger.LogInfo($"Successfully decrypted data, input: {value.Length} bytes, key: {key.Length} bytes.");
        
        if (opt.ValidateStartsWith != null)
        {
            var assert = EasyInput.TryPreProcess(opt.ValidateStartsWith).ToByteArray();
            if (assert.Length > value.Length)
            {
                _logger.LogErro($"Validate StartsWith error: The required pattern is longer than the value itself.");
            }
            else
            {
                bool res = true;
                for (int i = 0; i < assert.Length; i++)
                {
                    if (value[i] != assert[i])
                    {
                        res = false;
                        break;
                    }
                }

                if (res)
                {
                    _logger.LogInfo($"Validate StartsWith OK: Decrypted value starts with the provided pattern.");
                }
                else
                {
                    _logger.LogWarn($"Validate StartsWith failed: Decrypted value's start mismatches the provided pattern.");
                }
            }
        }
        if (opt.ValidateEndsWith != null)
        {
            var assert = EasyInput.TryPreProcess(opt.ValidateEndsWith).ToByteArray();
            if (assert.Length > value.Length)
            {
                _logger.LogErro($"Validate EndsWith error: The required pattern is longer than the value itself.");
            }
            else
            {
                bool res = true;
                var baseIndex = value.Length - assert.Length;
                for (int i = 0; i < assert.Length; i++)
                {
                    if (value[baseIndex + i] != assert[i])
                    {
                        res = false;
                        break;
                    }
                }

                if (res)
                {
                    _logger.LogInfo($"Validate EndsWith OK: Decrypted value ends with the provided pattern.");
                }
                else
                {
                    _logger.LogWarn($"Validate EndsWith failed: Decrypted value's end mismatches the provided pattern.");
                }
            }
        }
    }

    /// <summary>
    /// 将 <paramref name="bytes"/> 与 <paramref name="xorkey"/> 进行异或解密。
    /// </summary>
    /// <param name="bytes">需要解密的内容。 </param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    /// <param name="xorkey"></param>
    public static void XorDecrypt(Span<byte> bytes, byte[] xorkey, int offset = 0, int length = -1)
    {
        if (length < 0) length = bytes.Length - offset;
        else length = Math.Min(length, bytes.Length - offset);
        for (int i = offset; i < offset + length; i++)
        {
            bytes[i] = (byte)(bytes[i] ^ xorkey[i % xorkey.Length]);
        }
    }
}