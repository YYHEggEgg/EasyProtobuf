using CommandLine;
using System.Text;
using System.Security.Cryptography;
using TextCopy;
using XC.RSAUtil;
using YYHEggEgg.Shell;
using YYHEggEgg.Shell.Model;

namespace YYHEggEgg.EasyProtobuf.Commands;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
#region Option Base
internal class RsaKeyInputOptionBase
{
    [Value(0, Required = false, MetaName = "input-key-filePath", HelpText = "The path of input key file.")]
    public string InputKeyFilePath { get; set; }
    [Option("cb-in", Required = false, Default = false, HelpText = "Get input from the clipboard.")]
    public bool ClipboardInput { get; set; }

    public async Task<byte[]> GetKeyBytesAsync()
    {
        if (InputKeyFilePath != null) return File.ReadAllBytes(Path.GetFullPath(InputKeyFilePath));
        else if (ClipboardInput)
        {
            var keyBin = Encoding.UTF8.GetBytes(await ClipboardService.GetTextAsync() ?? "");
            if (keyBin.Length == 0)
            {
                throw new InvalidOperationException($"Can't get the key from clipboard!");
            }
            return keyBin;
        }
        else
        {
            throw new InvalidOperationException($"Please specify either the input file path or '--cb-in' option!");
        }
    }
}

internal class RsaEncryptionOptionBase : RsaOperationOptionBase
{
    [Option("padding", Required = false, MetaValue = "enc-padding", Default = "Pkcs1", HelpText = "The padding used in the RSA operation.")]
    public string PaddingString { get; set; }
    public RSAEncryptionPadding Padding
    {
        get
        {
            return PaddingString switch
            {
                nameof(RSAEncryptionPadding.OaepSHA1) => RSAEncryptionPadding.OaepSHA1,
                nameof(RSAEncryptionPadding.OaepSHA256) => RSAEncryptionPadding.OaepSHA256,
                nameof(RSAEncryptionPadding.OaepSHA384) => RSAEncryptionPadding.OaepSHA384,
                nameof(RSAEncryptionPadding.OaepSHA512) => RSAEncryptionPadding.OaepSHA512,
                nameof(RSAEncryptionPadding.Pkcs1) => RSAEncryptionPadding.Pkcs1,
                _ => throw new ArgumentException("The input param '--padding' isn't a valid value.")
            };
        }
    }
}

internal class RsaVerificationOptionBase : RsaOperationOptionBase
{
    [Option("padding", Required = false, MetaValue = "signing-padding", Default = "Pkcs1", HelpText = "The padding used in the RSA operation.")]
    public string PaddingString { get; set; }
    public RSASignaturePadding Padding
    {
        get
        {
            return PaddingString switch
            {
                nameof(RSASignaturePadding.Pkcs1) => RSASignaturePadding.Pkcs1,
                nameof(RSASignaturePadding.Pss) => RSASignaturePadding.Pss,
                _ => throw new ArgumentException("The input param '--padding' isn't a valid value.")
            };
        }
    }

    [Option("hash", Required = false, MetaValue = "hash-algorithm", Default = "SHA256", HelpText = "The hash algorithm used in generating the signature.")]
    public string HashAlgorithmString { get; set; }
    public HashAlgorithmName HashAlgorithm
    {
        get
        {
            return HashAlgorithmString switch
            {
                nameof(HashAlgorithmName.MD5) => HashAlgorithmName.MD5,
                nameof(HashAlgorithmName.SHA1) => HashAlgorithmName.SHA1,
                nameof(HashAlgorithmName.SHA256) => HashAlgorithmName.SHA256,
                nameof(HashAlgorithmName.SHA384) => HashAlgorithmName.SHA384,
                nameof(HashAlgorithmName.SHA512) => HashAlgorithmName.SHA512,
                _ => throw new ArgumentException("The input param '--hash' isn't a valid value.")
            };
        }
    }
}

internal class RsaOperationOptionBase
{
    [Value(0, Required = true, MetaName = "data", HelpText = "The data you want to operate on.")]
    public IEnumerable<string> BinaryData { get; set; }
    public byte[] Data => EasyInput.TryPreProcess(BinaryData).ToByteArray();

    [Option('k', "key", Required = true, MetaValue = "input-key-filePath", HelpText = "The path of input key file.")]
    public string InputKeyFilePath { get; set; }
    public RSAUtilBase GetRSAWorker()
    {
        return RSAUtilBase.LoadRSAKey(File.ReadAllBytes(InputKeyFilePath));
    }
}
#endregion

[Verb("keyconv", false, HelpText = "Convert the provided RSA key (PEM or XML) into any supported format.")]
internal class RsaKeyConvertOption : RsaKeyInputOptionBase
{
    [Option('o', "outkey", Required = true, MetaValue = "Key-Formats", HelpText = "The output key type you demand.")]
    public IEnumerable<string> OutputKeyType { get; set; }
    [Option('s', "save", Required = false, MetaValue = "save_path", Default = null, HelpText = "The path you want to save the output key to.")]
    public string? SaveTo { get; set; }
}

[Verb("get-keytype", false, HelpText = "Get the provided RSA key's format information.")]
internal class RsaGetKeyTypeOption : RsaKeyInputOptionBase
{
}

[Verb("keygen", false, HelpText = "Generate a RSA key.")]
internal class RsaKeyGenOption
{
    [Value(0, Required = true, MetaName = "Key-Formats", HelpText = "The output key type you demand.")]
    public IEnumerable<string> OutputKeyType { get; set; }
    [Option("save-pub", MetaValue = "path", Required = true, HelpText = "The path to save the generated public key.")]
    public string SavePublicTo { get; set; }
    [Option("save-pri", MetaValue = "path", Required = true, HelpText = "The path to save the generated private key.")]
    public string SavePrivateTo { get; set; }
    [Option("keysize", MetaValue = "size_bits", Required = true, HelpText = "The key size of the generated key (e.g. 2048 (bits)).")]
    public int KeySize { get; set; }
}

[Verb("encrypt", false, HelpText = "Encrypt the provided data with a Public Key.")]
internal class RsaEncryptOption : RsaEncryptionOptionBase
{
}

[Verb("decrypt", false, HelpText = "Decrypt the provided data with a Private Key.")]
internal class RsaDecryptOption : RsaEncryptionOptionBase
{
}

[Verb("sign", false, HelpText = "Generate the signature of provided data with a Private Key.")]
internal class RsaSignOption : RsaVerificationOptionBase
{
}

[Verb("verify", false, HelpText = "Verify the provided data and signature with a Public Key.")]
internal class RsaVerifyOption : RsaVerificationOptionBase
{
    [Option('s', "sign", MetaValue = "signature", Required = true, HelpText = "The signature of the provided RAW DATA.")]
    public IEnumerable<string> BinarySignature { get; set; }
    public byte[] Signature => EasyInput.TryPreProcess(BinarySignature).ToByteArray();
}
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

internal partial class RsaCmd : HasSubCommandsHandlerBase<RsaEncryptOption, RsaDecryptOption, RsaSignOption, RsaVerifyOption, RsaGetKeyTypeOption, RsaKeyGenOption, RsaKeyConvertOption>
{
    public override string CommandName => "rsa";

    public override string Description => "Perform RSA related operations.";

    protected override OptionHelpResult? CustomizeOptionHelpResult(OptionHelpResult help)
    {
        switch (help.MetaName)
        {
            case "enc-padding":
                help.AdditionalHelps =
                    [
                        "(Default: Pkcs1)",
                        "(Available: Pkcs1/OaepSHA1/OaepSHA256/OaepSHA284/OaepSHA512)",
                    ];
                break;
            case "signing-padding":
                help.AdditionalHelps =
                    [
                        "(Default: Pkcs1)",
                        "(Available: Pkcs1/Pss)",
                    ];
                break;
            case "hash-algorithm":
                help.AdditionalHelps =
                    [
                        "(Default: SHA256)",
                        "(Available: SHA256/MD5/SHA1/SHA384/SHA512)",
                    ];
                break;
            case "Key-Formats":
                help.AdditionalHelps =
                    [
                        "(Available: Public, Private, Xml, Pkcs1, Pkcs8, Der)",
                    ];
                break;
        }
        return help;
    }
}
