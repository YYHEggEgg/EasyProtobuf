using YYHEggEgg.EasyProtobuf.Commands.Dispatch;
using YYHEggEgg.EasyProtobuf.resLoader;
using Google.Protobuf;
using Newtonsoft.Json;
using YYHEggEgg.EasyProtobuf.Util;
using YYHEggEgg.EasyProtobuf.Configuration;

namespace YYHEggEgg.EasyProtobuf.Commands
{
    internal class DecryptCurrRegionCmd : CommandHandlerBase
    {
        public override string CommandName => "dcurr";

        public override string Description => "Decrypt query_cur_region content and verify it (to ensure it avaliable in anime game).";

        public override string Usage => $"dcurr <key_id> <curr_json>{Environment.NewLine}" +
            $"Decrypt and verify query_cur_region content, by the key from resources. {Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"Notice: <color=Yellow>If you're using Windows Terminal, press Ctrl+Alt+V to paste data with multiple lines (especially json data).</color>";

        public override async Task HandleAsync(string argList)
        {
            var conf = Config.Global.CurrRegionCmds;
            if (conf.BaseProto == null)
            {
                _logger.LogErro($"This command cannot be used because 'config.json/CurrRegionCmd/BasedProto' is not configured yet.");
                return;
            }

            var args = argList.Split(' ');
            uint key_id = uint.Parse(args[0]);
            var read = EasyInput.TryPreProcess(args, 1);
            if (read.InputType != EasyInputType.Json)
            {
                _logger.LogErro($"Input param 2 should be a valid json!");
            }
            (IMessage? currres, bool? verificationOK) = (null, null);
            try
            {
                (currres, verificationOK) = CurrExtend.GetCurrFromJson(conf.BaseProto, read.ProcessedString,
                    Resources.CPri[key_id], Resources.OfficialSPub[key_id]);
            }
            catch (JsonReaderException jex)
            {
                _logger.LogErro($"Decryption failed: {jex}");
                _logger.LogWarn($"It may because you provided false query_cur_region json.");
                return;
            }
            catch (KeyNotFoundException kex)
            {
                _logger.LogErro($"Decryption failed: {kex}");
                _logger.LogWarn($"It may because you provided false query_cur_region json.");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogErroTrace(ex, $"Decryption failed. ");
                _logger.LogWarn($"It may because the RSA key doesn't match " +
                    $"or you provided false query_cur_region json.");
                return;
            }
            var res = JsonFormatter.Default.Format(currres);
            if (string.IsNullOrWhiteSpace(res)) res = "<empty content or json/protobuf format failure>";
            _logger.LogInfo($"Decrypted json content: \n{res}");
            if (verificationOK == true)
            {
                _logger.LogInfo($"Sign Verified OK!");
            }
            else if (verificationOK == false)
            {
                _logger.LogWarn($"RSA Verification failed. " +
                    $"You may check whether a correct RSA key is configured.");
            }
            await Tools.SetClipBoardAsync(res);
        }
    }

    internal class GenerateCurrRegionCmd : CommandHandlerBase
    {
        public override string CommandName => "gencur";

        public override string Description => "Generate query_cur_region content and signature.";

        public override string Usage => $"gencur <key_id> <protobuf_content>{Environment.NewLine}" +
            $"Encrypt and sign query_cur_region content, by the key from resources.";

        public override async Task HandleAsync(string argList)
        {
            var conf = Config.Global.CurrRegionCmds;
            if (conf.BaseProto == null)
            {
                _logger.LogErro($"This command cannot be used because 'config.json/CurrRegionCmd/BasedProto' is not configured yet.");
                return;
            }

            var args = argList.Split(' ');
            uint key_id = uint.Parse(args[0]);
            var read = EasyInput.TryPreProcess(args, 1);
            if (read.InputType != EasyInputType.Json)
            {
                _logger.LogErro($"Input param 2 should be a valid json!");
            }
            IMessage? curr = null;
            try
            {
                curr = ProtoSerialize.Serialize(conf.BaseProto, read.ProcessedString ?? string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogErroTrace(ex, 
                    $"Protobuf serialization failed. ");
                _logger.LogWarn($"It may because the json isn't valid." +
                    $"It's recommended to modify based on the result" +
                    $"from json protobuf from 'util dcurr' command.");
                return;
            }
            if (curr == null)
            {
                _logger.LogErro($"Protobuf serialization failed (no exceptions thrown).");
                return;
            }

            try
            {
                var res = curr.GetCurrJson(Resources.CPri[key_id], Resources.LocalSPri[key_id]);
                _logger.LogInfo($"Result: \n{res}");
                await Tools.SetClipBoardAsync(res);
            }
            catch (Exception ex)
            {
                _logger.LogErroTrace(ex, $"RSA encryption failed. ");
                _logger.LogWarn($"It may because you don't provide match key" +
                    $"in resources/ClientPri and resources/ServerPri.");
                return;
            }
        }
    }
}
