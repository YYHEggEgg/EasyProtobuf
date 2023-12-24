using YYHEggEgg.EasyProtobuf.resLoader;
using System.Buffers.Binary;
using System.Security.Cryptography;
using YYHEggEgg.EasyProtobuf.Util;
using YSFreedom.Common.Util;
using CommandLine;
using YYHEggEgg.Logger;

namespace YYHEggEgg.EasyProtobuf.Commands;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
internal class MT19937GenKeyOptionBase
{
    [Option("anime", Default = false, Required = false)]
    public bool UseAnimeImpl { get; set; }
    [Option("sleep", Default = false, Required = false)]
    public bool UseSleepImpl { get; set; }
    [Option("💤", Default = false, Required = false)]
    public bool UseSleeeepImpl { get; set; }

    public virtual bool ReportAvaliableOption(LoggerChannel log)
    {
        UseSleepImpl |= UseSleeeepImpl;
        if (UseAnimeImpl && UseSleepImpl)
        {
            log.LogErro($"Please specify only one MT implemention in '--anime' or '--sleep'!");
            return false;
        }
        if (!UseAnimeImpl && !UseSleepImpl)
        {
            log.LogErro($"Please specify an MT implemention in '--anime' or '--sleep'!");
            return false;
        }
        return true;
    }
}

[Verb("from-seed", false, HelpText = "Generate a 4096-bytes XOR Key from given final seed directly (result of clientRandKey xor serverRandKey).")]
internal class MT19937DirectGenOption : MT19937GenKeyOptionBase
{
    [Value(0, Required = true, HelpText = "The Input UInt64 / HEX Seed.")]
    public string InputSeed { get; set; }
}

[Verb("rsa", true, HelpText = "Generate a 4096-bytes XOR Key from RSA Encrypted clientRandKey and serverRandKey.")]
internal class MT19937FromRSAOption : MT19937GenKeyOptionBase
{
    [Value(0, Required = true, HelpText = "RSA Encrypted ClientRandKey Base64/HEX.")]
    public string ClientRandKey { get; set; }
    [Value(1, Required = true, HelpText = "RSA Encrypted ServerRandKey Base64/HEX.")]
    public string ServerRandKey { get; set; }
    [Option('k', "key", Required = true, HelpText = "The id of the RSA Key Pair you want to use.")]
    public uint KeyId { get; set; }
}
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

internal class MT19937Cmd : CommandHandlerBase
{
    public override string CommandName => "mt19937";

    public override string Description => "Generate 4096-byte XOR key with a certain UInt64 seed, " +
        "or with RSA param clientRandkey and serverRandKey.";

    public override string Usage => $"mt19937 [command] <args>{Environment.NewLine}" +
        $"  command rsa (default): Generate a 4096-bytes XOR Key from RSA Encrypted clientRandKey and serverRandKey.{Environment.NewLine}" +
        $"    mt19937 <--anime|--sleep> {Environment.NewLine}" +
        $"            -k, --key <rsa_key_id> {Environment.NewLine}" +
        $"            <RSA_Encrypted_clientRandKey_base64/hex> <RSA_Encrypted_serverRandKey_base64/hex> {Environment.NewLine}" +
        $"{Environment.NewLine}" +
        $"  command from-seed: Generate a 4096-bytes XOR Key from given final seed directly (result of clientRandKey xor serverRandKey).{Environment.NewLine}" +
        $"    mt19937 from-seed <--anime|--sleep> {Environment.NewLine}" +
        $"                      <uint64_seed|uint64_seed_HEX> {Environment.NewLine}" +
        $"{Environment.NewLine}" +
        $"Notice: <color=Yellow>If you're using Windows Terminal, press Ctrl+Alt+V to paste data with multiple lines.</color>";

    public override async Task HandleAsync(string argList)
    {
        var args = ParseAsArgs(argList);
        await DefaultCommandsParser.ParseArguments<MT19937DirectGenOption, MT19937FromRSAOption>(args)
            .MapResult(
                async (MT19937DirectGenOption o) => await HandleDirectGenAsync(o),
                async (MT19937FromRSAOption o) => await HandleFromRSAAsync(o),
                error =>
                {
                    OutputInvalidUsage(error);
                    ShowUsage();
                    return Task.CompletedTask;
                });
    }

    private Task HandleFromRSAAsync(MT19937FromRSAOption o)
    {
        if (!o.ReportAvaliableOption(_logger)) return Task.CompletedTask;

        uint key_id = o.KeyId;
        ulong seed = 0;
        var server_rand_key = Resources.CPri[key_id].RsaDecrypt(
            EasyInput.TryPreProcess(o.ServerRandKey).ToByteArray(), RSAEncryptionPadding.Pkcs1);
        var client_rand_key = Resources.LocalSPri[key_id].RsaDecrypt(
            EasyInput.TryPreProcess(o.ClientRandKey).ToByteArray(), RSAEncryptionPadding.Pkcs1);
        client_rand_key = client_rand_key.Fill0(8);
        server_rand_key = server_rand_key.Fill0(8);
        if (o.UseAnimeImpl)
        {
            seed = BinaryPrimitives.ReadUInt64BigEndian(client_rand_key) ^ BinaryPrimitives.ReadUInt64BigEndian(server_rand_key);
        }
        else if (o.UseSleepImpl)
        {
            seed = BinaryPrimitives.ReadUInt64LittleEndian(client_rand_key) ^ BinaryPrimitives.ReadUInt64LittleEndian(server_rand_key);
        }

        LogKeyFromSeed(o, seed);
        return Task.CompletedTask;
    }

    private Task HandleDirectGenAsync(MT19937DirectGenOption o)
    {
        if (!ulong.TryParse(o.InputSeed, out ulong seed))
        {
            if (o.InputSeed.StartsWith("0x") || o.InputSeed.StartsWith("0X"))
                o.InputSeed = o.InputSeed[2..];
            seed = Convert.FromHexString(o.InputSeed).Fill0(8).GetUInt64(0);
        }

        LogKeyFromSeed(o, seed);
        return Task.CompletedTask;
    }

    private void LogKeyFromSeed(MT19937GenKeyOptionBase mode, ulong seed)
    {
        if (!mode.ReportAvaliableOption(_logger)) return;

        if (mode.UseAnimeImpl)
        {
            _logger.LogInfo($"MT64 result:{Environment.NewLine}-----BEGIN HEX 4096 Xor Key-----{Environment.NewLine}" +
                Convert.ToHexString(Tools.Generate4096KeyByMT19937_Anime(seed)) +
                $"{Environment.NewLine}-----END HEX 4096 Xor Key-----");
        }
        else
        {
            _logger.LogInfo($"MT64 result:{Environment.NewLine}-----BEGIN HEX 4096 Xor Key-----{Environment.NewLine}" +
                Convert.ToHexString(Tools.Generate4096KeyByMT19937_Sleep(seed)) +
                $"{Environment.NewLine}-----END HEX 4096 Xor Key-----");
        }
    }
}
