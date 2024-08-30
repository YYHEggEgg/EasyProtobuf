using YYHEggEgg.EasyProtobuf.Commands.Dispatch;
using YYHEggEgg.EasyProtobuf.resLoader;
using Google.Protobuf;
using Newtonsoft.Json;
using YYHEggEgg.EasyProtobuf.Util;
using YYHEggEgg.EasyProtobuf.Configuration;
using YYHEggEgg.Logger;
using YYHEggEgg.Shell;
using Microsoft.Extensions.Logging;
using YYHEggEgg.EasyProtobuf.MainCLI;

namespace YYHEggEgg.EasyProtobuf.Commands
{
    internal class DecryptCurrRegionCmd : CommandHandlerBase
    {
        public override string CommandName => "dcurr";

        public override string Description => "Decrypt query_cur_region content and verify it (to ensure it avaliable in anime game).";

        public override IEnumerable<string> UsageLines =>
            [
                $"dcurr <key_id> <curr_json>",
                $"Decrypt and verify query_cur_region content, by the key from resources.",
                "",
                "Notice: <color=Yellow>If you're using Windows Terminal, press Ctrl+Alt+V to paste data with multiple lines (especially json data).</color>"
            ];

        public override async Task<bool> HandleAsync(string argList, CancellationToken cancellationToken)
        {
            var conf = Config.Global.CurrRegionCmds;
            if (conf.UseProtoCurr && conf.BasedProto == null)
            {
                _logger.LogError($"This command cannot be used because 'config.json/CurrRegionCmd/BasedProto' is not configured yet.");
                return false;
            }

            var args = argList.Split(' ');
            uint key_id = uint.Parse(args[0]);
            var read = EasyInput.TryPreProcess(args, 1);
            if (read.InputType != EasyInputType.Json)
            {
                _logger.LogError($"Input param 2 should be a valid json!");
            }
            string? res = null;
            bool? verificationOK;
            try
            {
                if (conf.UseProtoCurr)
                {
#pragma warning disable CS8604 // Checked: L24 conf.UseProtoCurr && conf.BasedProto == null -> return
                    (IMessage? currres, verificationOK) = CurrExtend.GetCurrFromJson(conf.BasedProto, read.ProcessedString,
                        Resources.CPri[key_id], Resources.OfficialSPub[key_id], _logger);
#pragma warning restore CS8604
                    res = JsonFormatter.Default.Format(currres);
                }
                else
                {
                    (res, verificationOK) = CurrExtend.GetJsonCurrFromJson(read.ProcessedString,
                        Resources.CPri[key_id], Resources.OfficialSPub[key_id]);
                }
            }
            catch (JsonReaderException jex)
            {
                _logger.LogError(jex, $"Decryption failed.");
                _logger.LogWarning($"It may because you provided a bad-formatted json.");
                return false;
            }
            catch (KeyNotFoundException kex)
            {
                _logger.LogError(kex, $"Decryption failed.");
                _logger.LogWarning($"It may because you requested keys that haven't been placed in resources.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Decryption failed.");
                _logger.LogWarning($"It may because the RSA key doesn't match " +
                    $"or you provided false query_cur_region json.");
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(res)) res = "<empty content or json/protobuf format failure>";
            _logger.LogInformation("Decrypted json content: \n{res}", res);
            if (verificationOK == true)
            {
                _logger.LogInformation($"Sign Verified OK!");
            }
            else if (verificationOK == false)
            {
                _logger.LogWarning($"RSA Verification failed. " +
                    $"You may check whether a correct RSA key is configured.");
            }
            await Tools.SetClipBoardAsync(res);
            return true;
        }

        private CurrRegionCmdsAutoCompleteHandler _autoCmplHandler = new(true);
        public override SuggestionResult GetSuggestions(string text, int index)
        {
            return _autoCmplHandler.GetSuggestions(text, index);
        }
    }

    internal class GenerateCurrRegionCmd : CommandHandlerBase
    {
        public override string CommandName => "gencur";

        public override string Description => "Generate query_cur_region content and signature.";

        public override IEnumerable<string> UsageLines =>
            [
                "gencur <key_id> <protobuf_content>",
                "Encrypt and sign query_cur_region content, by the key from resources.",
            ];

        public override async Task<bool> HandleAsync(string argList, CancellationToken cancellationToken)
        {
            var conf = Config.Global.CurrRegionCmds;
            if (conf.UseProtoCurr && conf.BasedProto == null)
            {
                _logger.LogError($"This command cannot be used because 'config.json/CurrRegionCmd/BasedProto' is not configured yet.");
                return false;
            }

            var args = argList.Split(' ');
            uint key_id = uint.Parse(args[0]);
            var read = EasyInput.TryPreProcess(args, 1);
            if (read.InputType != EasyInputType.Json)
            {
                _logger.LogError($"Input param 2 should be a valid json!");
            }
            string? res = null;
            try
            {
                if (conf.UseProtoCurr)
                {
#pragma warning disable CS8604 // Checked: L102 conf.UseProtoCurr && conf.BasedProto == null -> return
                    res = ProtobufHandler.Serialize(conf.BasedProto, read.ProcessedString ?? string.Empty)
                        ?.GetCurrJson(Resources.CPri[key_id], Resources.LocalSPri[key_id]);
#pragma warning restore CS8604
                }
                else
                {
                    res = CurrExtend.GetJsonFromCurrJson(read.ProcessedString ?? string.Empty, Resources.CPri[key_id], Resources.LocalSPri[key_id]);
                }
            }
            catch (JsonReaderException jex)
            {
                _logger.LogError(jex, "Encryption failed.");
                _logger.LogWarning($"It may because you provided a bad-formatted json.");
                return false;
            }
            catch (KeyNotFoundException kex)
            {
                _logger.LogError(kex, $"Encryption failed.");
                _logger.LogWarning($"It may because you requested keys that haven't been placed in resources.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Protobuf serialization / JSON read failed.");
                _logger.LogWarning($"It may because the json isn't valid." +
                    $"It's recommended to modify based on the result" +
                    $"from json protobuf from 'util dcurr' command.");
                return false;
            }
            if (res == null)
            {
                _logger.LogError($"Protobuf serialization / JSON read failed (no exceptions thrown).");
                return false;
            }

            try
            {
                _logger.LogInformation("Result: \n{res}", res);
                await Tools.SetClipBoardAsync(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RSA encryption failed.");
                _logger.LogWarning($"It may because you don't provide match key" +
                    $"in resources/ClientPri and resources/ServerPri.");
                return false;
            }
            return true;
        }

        private CurrRegionCmdsAutoCompleteHandler _autoCmplHandler = new(false);
        public override SuggestionResult GetSuggestions(string text, int index)
        {
            return _autoCmplHandler.GetSuggestions(text, index);
        }
    }
}
