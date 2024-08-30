using CommandLine;
using Microsoft.Extensions.Logging;
using YYHEggEgg.EasyProtobuf.Util;
using YYHEggEgg.Shell;

namespace YYHEggEgg.EasyProtobuf.Commands;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
[Verb("set-key", false, HelpText = "Set the default key for XOR operations.")]
internal class XorSetDefaultKeyOption
{
    [Value(0, Required = true, MetaName = "xorkey", HelpText = "The demanded default key for XOR command.")]
    public IEnumerable<string> Key { get; set; }
}

[Verb("operate", true, HelpText = "Do a XOR operation.")]
internal class XorOperateOption
{
    [Option('k', "xorkey", MetaValue = "bin", Required = false, Default = null, HelpText = "Set the default key for XOR command.")]
    public IEnumerable<string>? Key { get; set; }
    [Value(0, Required = true, MetaName = "value", HelpText = "The HEX / Base64 Content that should be decrypted.")]
    public IEnumerable<string> Value { get; set; }
    [Option("validate-startswith", MetaValue = "bin", Required = false, Default = null, HelpText = "Validate the result starts with a certain pattern.")]
    public IEnumerable<string>? ValidateStartsWith { get; set; }
    [Option("validate-endswith", MetaValue = "bin", Required = false, Default = null, HelpText = "Validate the result ends with a certain pattern.")]
    public IEnumerable<string>? ValidateEndsWith { get; set; }
}
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

internal class XorCmd : HasSubCommandsHandlerBase<XorOperateOption, XorSetDefaultKeyOption>
{
    public override string CommandName => "xor";

    public override string Description => "Perform the XOR decryption and analyze the result.";

    protected override IEnumerable<string>? AdditionalDescLines =>
        [
            EasyInput.MultipleInputNotice,
            "",
            "",
            "Notice: <color=Yellow>If you're using Windows Terminal, press Ctrl+Alt+V to paste data with multiple lines.</color>"
        ];

    private byte[]? default_key = null;

    public override Task<bool> HandleAsync(XorSetDefaultKeyOption opt, CancellationToken cancellationToken)
    {
        default_key = EasyInput.TryPreProcess(opt.Key).ToByteArray();
        _logger.LogInformation("Successfully set default key: {length} bytes.", default_key.Length);
        return Task.FromResult(true);
    }

    public override async Task<bool> HandleAsync(XorOperateOption opt, CancellationToken cancellationToken)
    {
        byte[]? key = default_key;
        if (opt.Key != null && opt.Key.Any()) key = EasyInput.TryPreProcess(opt.Key).ToByteArray();
        if (key == null)
        {
            _logger.LogError("Please give the using XOR key by '-k' option, or set the default key with 'xor set-key' command.");
            return false;
        }

        var value = EasyInput.TryPreProcess(opt.Value).ToByteArray();
        XorDecrypt(value, key);
        await Tools.SetClipBoardAsync(Convert.ToHexString(value));
        _logger.LogInformation("Successfully decrypted data, input: {length} bytes, key: {length} bytes.", value.Length, key.Length);
        
        if (opt.ValidateStartsWith != null && opt.ValidateStartsWith.Any())
        {
            var assert = EasyInput.TryPreProcess(opt.ValidateStartsWith).ToByteArray();
            if (assert.Length > value.Length)
            {
                _logger.LogError($"Validate StartsWith error: The required pattern is longer than the value itself.");
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
                    _logger.LogInformation($"Validate StartsWith OK: Decrypted value starts with the provided pattern.");
                }
                else
                {
                    _logger.LogWarning($"Validate StartsWith failed: Decrypted value's start mismatches the provided pattern.");
                }
            }
        }
        if (opt.ValidateEndsWith != null && opt.ValidateEndsWith.Any())
        {
            var assert = EasyInput.TryPreProcess(opt.ValidateEndsWith).ToByteArray();
            if (assert.Length > value.Length)
            {
                _logger.LogError($"Validate EndsWith error: The required pattern is longer than the value itself.");
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
                    _logger.LogInformation($"Validate EndsWith OK: Decrypted value ends with the provided pattern.");
                }
                else
                {
                    _logger.LogWarning($"Validate EndsWith failed: Decrypted value's end mismatches the provided pattern.");
                }
            }
        }
        return true;
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