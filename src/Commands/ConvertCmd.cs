using System.Text;
using CommandLine;
using YYHEggEgg.EasyProtobuf.Util;

namespace YYHEggEgg.EasyProtobuf.Commands
{
    internal class ConvertOptions
    {
        [Value(0, Required = true)]
        public IEnumerable<string>? Data { get; set; }
        [Option('t', "target", Required = false, Default = EasyInputType.IdentifyFailure)]
        public EasyInputType ConvertTarget { get; set; }
    }

    internal class ConvertCmd : StandardCommandHandler<ConvertOptions>
    {
        public override string CommandName => "convert";

        public override string Description => "Automatically convert data between base64 and HEX format.";

        public override string Usage => $"convert <base64_data/hex_data>{Environment.NewLine}" +
            $"  [-t <Base64|Hex|Json>] {Environment.NewLine}" +
            EasyInput.MultipleInputNotice +
            $"{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"Notice: <color=Yellow>If you're using Windows Terminal, press Ctrl+Alt+V to paste data with multiple lines.</color>";

        private EasyInputType GetOutputType(ConvertOptions o, EasyInputResult res)
        {
            if (o.ConvertTarget != EasyInputType.IdentifyFailure) return o.ConvertTarget;
            switch (res.InputType)
            {
                case EasyInputType.Base64:
                    return EasyInputType.Hex;
                case EasyInputType.Hex:
                    return EasyInputType.Base64;
                default:
                    _logger.LogErro("Can't guess the convert target type. Please specify '-t' param.");
                    return EasyInputType.IdentifyFailure;
            }
        }

        public override async Task HandleAsync(ConvertOptions o)
        {
            EasyInputResult res = EasyInput.TryPreProcess(o.Data ?? []);
            var outputType = GetOutputType(o, res);
            if (outputType == EasyInputType.IdentifyFailure) return;
            byte[] bytes = res.ToByteArray(true);
            switch (outputType)
            {
                case EasyInputType.Hex:
                    _logger.LogInfo($"Converted to HEX format, handled {bytes.Length} bytes.");
                    await Tools.SetClipBoardAsync(Convert.ToHexString(bytes));
                    break;
                case EasyInputType.Base64:
                    _logger.LogInfo($"Converted to Base64 format, handled {bytes.Length} bytes.");
                    await Tools.SetClipBoardAsync(Convert.ToBase64String(bytes));
                    break;
                case EasyInputType.Json:
                    var json = Tools.ConvertJsonString(Encoding.UTF8.GetString(bytes));
                    _logger.LogInfo($"Converted to JSON format.");
                    await Tools.SetClipBoardAsync(json);
                    break;
                default:
                    _logger.LogErro($"Input type is not supported!");
                    break;
            }
        }
    }
}
