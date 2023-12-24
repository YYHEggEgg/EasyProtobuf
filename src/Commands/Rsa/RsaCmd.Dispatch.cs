using System.Security.Cryptography;
using CommandLine;
using XC.RSAUtil;

namespace YYHEggEgg.EasyProtobuf.Commands;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
[Verb("keyconv", false, HelpText = "Convert the provided RSA key (PEM or XML) into any supported format.")]
internal class RsaKeyConvertOption
{
    [Value(0, Required = false, HelpText = "The path of input key file.")]
    public string InputKeyFilePath { get; set; }
    [Option("cb-in", Required = false, Default = false, HelpText = "Get input from the clipboard.")]
    public bool ClipboardInput { get; set; }
    [Option('o', "outkey", Required = true, HelpText = "The output key type you demand.")]
    public IEnumerable<string> OutputKeyType { get; set; }
}

internal class RsaOperationOptionBase
{
    [Value(0, Required = true, HelpText = "The data you want to operate on.")]
    public IEnumerable<string> BinaryData { get; set; }
    public byte[] Data => EasyInput.TryPreProcess(BinaryData).ToByteArray();

    [Option('k', "key", Required = true, HelpText = "The path of input key file.")]
    public string InputKeyFilePath { get; set; }
    public RSAUtilBase GetRSAWorker()
    {
        return RSAUtilBase.LoadRSAKey(File.ReadAllText(InputKeyFilePath));
    }
}

internal class RsaEncryptionOptionBase : RsaOperationOptionBase
{
    [Option("padding", Required = false, Default = "Pkcs1", HelpText = "The padding used in the RSA operation.")]
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

[Verb("encrypt", false, HelpText = "Encrypt the provided data with a Public Key.")]
internal class RsaEncryptOption : RsaEncryptionOptionBase
{
}

[Verb("decrypt", false, HelpText = "Decrypt the provided data with a Private Key.")]
internal class RsaDecryptOption : RsaEncryptionOptionBase
{
}

internal class RsaVerificationOptionBase : RsaOperationOptionBase
{
    [Option("padding", Required = false, Default = "Pkcs1", HelpText = "The padding used in the RSA operation.")]
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

    [Option("hash", Required = false, Default = "SHA256", HelpText = "The hash algorithm used in generating the signature.")]
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

[Verb("sign", false, HelpText = "Generate the signature of provided data with a Private Key.")]
internal class RsaSignOption : RsaVerificationOptionBase
{
}

[Verb("verify", false, HelpText = "Verify the provided data and signature with a Public Key.")]
internal class RsaVerifyOption : RsaVerificationOptionBase
{
    [Option('s', "sign", Required = true, HelpText = "The signature of the provided RAW DATA.")]
    public IEnumerable<string> BinarySignature { get; set; }
    public byte[] Signature => EasyInput.TryPreProcess(BinarySignature).ToByteArray();
}
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

internal partial class RsaCmd : CommandHandlerBase
{
    public override string CommandName => "rsa";

    public override string Description => "Perform RSA related operations.";

    public override string Usage => $"rsa [command] <args> {Environment.NewLine}" +
    $"  command encrypt: Encrypt the provided data with a Public Key. {Environment.NewLine}" +
    $"    rsa encrypt <raw_data>                      The data you want to encrypt. {Environment.NewLine}" +
    $"                -k, --key <input-key-filePath>  The path of input key file. {Environment.NewLine}" +
    $"                --padding [padding]             The padding used in the RSA operation.  {Environment.NewLine}" +
    $"                                                (Default: Pkcs1) {Environment.NewLine}" +
    $"                                                (Avaliable: Pkcs1/OaepSHA1/OaepSHA256/OaepSHA284/OaepSHA512) {Environment.NewLine}" +
    $" {Environment.NewLine}" +
    $"  command decrypt: Decrypt the provided data with a Private Key. {Environment.NewLine}" +
    $"    rsa decrypt <enc_data>                      The data you want to decrypt. {Environment.NewLine}" +
    $"                -k, --key <input-key-filePath>  The path of input key file. {Environment.NewLine}" +
    $"                --padding [padding]             The padding used in the RSA operation. {Environment.NewLine}" +
    $"                                                (Default: Pkcs1) {Environment.NewLine}" +
    $"                                                (Avaliable: Pkcs1/OaepSHA1/OaepSHA256/OaepSHA284/OaepSHA512) {Environment.NewLine}" +
    $" {Environment.NewLine}" +
    $"  command sign: Generate the signature of provided data with a Private Key. {Environment.NewLine}" +
    $"    rsa sign <raw_data>                      The data you want to sign. {Environment.NewLine}" +
    $"             -k, --key <input-key-filePath>  The path of input key file. {Environment.NewLine}" +
    $"             --hash [hash-algorithm]         The hash algorithm used in generating the signature. {Environment.NewLine}" +
    $"                                             (Default: SHA256) {Environment.NewLine}" +
    $"                                             (Avaliable: SHA256/MD5/SHA1/SHA384/SHA512) {Environment.NewLine}" +
    $"             --padding [padding]             The padding used in the RSA operation. {Environment.NewLine}" +
    $"                                             (Default: Pkcs1) {Environment.NewLine}" +
    $"                                             (Avaliable: Pkcs1/Pss) {Environment.NewLine}" +
    $" {Environment.NewLine}" +
    $"  command verify: Verify the provided data and signature with a Public Key. {Environment.NewLine}" +
    $"    rsa verify <raw_data>                      The RAW DATA you want to verify. {Environment.NewLine}" +
    $"               -k, --key <input-key-filePath>  The path of input key file. {Environment.NewLine}" +
    $"               -s, --sign <signature>          The signature of the provided RAW DATA. {Environment.NewLine}" +
    $"               --hash [hash-algorithm]         The hash algorithm used in generating the signature. {Environment.NewLine}" +
    $"                                               (Default: SHA256) {Environment.NewLine}" +
    $"                                               (Avaliable: SHA256/MD5/SHA1/SHA384/SHA512) {Environment.NewLine}" +
    $"               --padding [padding]             The padding used in the RSA operation.  {Environment.NewLine}" +
    $"                                               (Default: Pkcs1)  {Environment.NewLine}" +
    $"                                               (Avaliable: Pkcs1/Pss) {Environment.NewLine}" +
    $" {Environment.NewLine}" +
    $"  command keyconv: Convert the provided RSA key (PEM or XML) into any supported format. {Environment.NewLine}" +
    $"    rsa keyconv <input-key-filePath>        The path of input key file. {Environment.NewLine}" +
    $"                (or --cb-in: Get input from the clipboard.) {Environment.NewLine}" +
    $"                -o, --outkey [Key-Formats]  The output key type you demand. {Environment.NewLine}" +
    $"                                            (Avaliable: Public, Private, Xml, Pkcs1, Pkcs8)";

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<RsaEncryptOption, RsaDecryptOption, RsaSignOption, RsaVerifyOption, RsaKeyConvertOption>(args)
            .MapResult(
                async (RsaEncryptOption opt) => await HandleEncryptAsync(opt),
                async (RsaDecryptOption opt) => await HandleDecryptAsync(opt),
                async (RsaSignOption opt) => await HandleSignAsync(opt),
                async (RsaVerifyOption opt) => await HandleVerifyAsync(opt),
                async (RsaKeyConvertOption opt) => await HandleKeyConvertAsync(opt),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }
}
