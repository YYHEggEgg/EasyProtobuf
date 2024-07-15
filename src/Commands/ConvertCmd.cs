using System.Text;
using CommandLine;
using Microsoft.Extensions.Logging;
using YYHEggEgg.EasyProtobuf.Util;
using YYHEggEgg.Shell;

namespace YYHEggEgg.EasyProtobuf.Commands
{
    internal class ConvertOptions
    {
        [Value(0, Required = true, MetaName = "base64_data/hex_data")]
        public IEnumerable<string>? Data { get; set; }
        [Option('t', "target", MetaValue = "Base64|Hex|Json", Required = false, Default = EasyInputType.IdentifyFailure)]
        public EasyInputType ConvertTarget { get; set; }
    }

    internal class ConvertCmd : StandardCommandHandler<ConvertOptions>
    {
        public override string CommandName => "convert";

        public override string Description => "Automatically convert data between base64 and HEX format.";

        protected override IEnumerable<string>? AdditionalDescLines =>
            [
                EasyInput.MultipleInputNotice,
                "",
                "",
                $"Notice: <color=Yellow>If you're using Windows Terminal, press Ctrl+Alt+V to paste data with multiple lines.</color>",
            ];

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
                    _logger.LogError("Can't guess the convert target type. Please specify '-t' param.");
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
                    _logger.LogInformation("Converted to HEX format, handled {len} bytes.", bytes.Length);
                    await Tools.SetClipBoardAsync(Convert.ToHexString(bytes));
                    break;
                case EasyInputType.Base64:
                    _logger.LogInformation("Converted to Base64 format, handled {len} bytes.", bytes.Length);
                    await Tools.SetClipBoardAsync(Convert.ToBase64String(bytes));
                    break;
                case EasyInputType.Json:
                    var json = Tools.ConvertJsonString(Encoding.UTF8.GetString(bytes));
                    _logger.LogInformation("Converted to JSON format.");
                    await Tools.SetClipBoardAsync(json);
                    break;
                default:
                    _logger.LogError("Input type is not supported!");
                    break;
            }
        }
    }
}
