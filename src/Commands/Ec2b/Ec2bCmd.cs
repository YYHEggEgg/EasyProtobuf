using AssetLib.Formats;
using YYHEggEgg.EasyProtobuf.Util;

namespace YYHEggEgg.EasyProtobuf.Commands
{
    internal class Ec2bCmd : CommandHandlerBase
    {
        public override string CommandName => "ec2b";

        public override string Description => "Make operations on dispatch secret_key/secret_seed.";

        public override string Usage => $"ec2b get_key <content_bindata(base64/hex)>{Environment.NewLine}" +
            EasyInput.MultipleInputNotice +
            $"{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"Notice: <color=Yellow>If you're using Windows Terminal, press Ctrl+Alt+V to paste data with multiple lines.</color>{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"ec2b encrypt <uint64_t_seed>";

        public override void CleanUp()
        {
            throw new NotImplementedException();
        }

        public override async Task HandleAsync(string argList)
        {
            var args = ParseAsArgs(argList);
            if (args.Count <= 1)
            {
                _logger.LogErro($"Too few params! Usage: {Usage}");
                return;
            }
            switch (args[0])
            {
                case "get_key":
                    var read = EasyInput.TryPreProcess(args, 1);
                    if (read.InputType != EasyInputType.Base64 
                        && read.InputType != EasyInputType.Hex)
                    {
                        _logger.LogErro($"The input type isn't supported!");
                        return;
                    }
                    var hexkey = Convert.ToHexString(Ec2b.Decrypt(read.ToByteArray()));
                    _logger.LogInfo(hexkey);
                    await Tools.SetClipBoardAsync(hexkey);
                    break;
                case "encrypt":
                    var hexseed = Convert.ToHexString(Ec2b.Encrypt(ulong.Parse(args[1])));
                    _logger.LogInfo(hexseed);
                    await Tools.SetClipBoardAsync(hexseed);
                    break;
                default:
                    _logger.LogErro($"Usage: {Usage}");
                    break;
            }
        }
    }
}
