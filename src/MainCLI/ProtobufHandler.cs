using CommandLine;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using YYHEggEgg.EasyProtobuf.Configuration;
using YYHEggEgg.EasyProtobuf.Util;
using YYHEggEgg.Logger;
using YYHEggEgg.Shell;
using YYHEggEgg.Shell.Attributes;

namespace YYHEggEgg.EasyProtobuf.MainCLI;

internal class ProtobufOption
{
    [Value(0, Required = true)]
    public string Protoname { get; set; } = null!;
}

[DoNotRegisterCommand]
internal class ProtobufHandler : StandardCommandHandler<ProtobufOption>
{
    public override string CommandName => nameof(ProtobufHandler);
    public override string Description => throw new NotImplementedException();

    public override async Task HandleAsync(ProtobufOption opt)
    {
        var protoname = opt.Protoname;
        Type? prototype = null;
        try
        {
            prototype = FindProtoMessageType(protoname);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Find Proto (by name) failed.");
        }
        if (prototype == null)
        {
            _logger.LogError("Proto type or command not found: '{protoname}'.", protoname);
            return;
        }
        _logger.LogInformation("Well done! The proto exists.");

        _logger.LogInformation("Please type base64 encoded or HEX protobuf bin data (auto detect):");
        _logger.LogInformation("You can also paste json data to get its serialized data.");

        _logger.LogInformation(EasyInput.MultipleInputNotice);
        var raw_text = await ConsoleWrapper.ReadLineAsync(false);
        var res = EasyInput.TryPreProcess(raw_text);

        string? stroutput = null;
        IMessage? msg = null;
        switch (res.InputType)
        {
            case EasyInputType.Base64:
            case EasyInputType.Hex:
                var bytes = res.ToByteArray();
                msg = ProtoSerialize.Deserialize(prototype, bytes);
                if (msg == null)
                {
                    _logger.LogInformation($"Deserialized message = null");
                    _logger.LogWarning("Serialization/Deserialization probably failed!");
                    return;
                }
                stroutput = JsonFormatter.Default.Format(msg);
                _logger.LogInformation("Converted Json:{newline}{output}", Environment.NewLine, stroutput);
                break;
            case EasyInputType.Json:
                msg = ProtoSerialize.Serialize(prototype, res.ProcessedString ?? string.Empty);

                if (msg == null)
                {
                    _logger.LogInformation("Serialized message = null");
                    _logger.LogWarning("Serialization/Deserialization probably failed!");
                    return;
                }
                stroutput = Convert.ToBase64String(msg.ToByteArray());
                _logger.LogInformation("Serialized Base64:{newline}{output}", Environment.NewLine, stroutput);
                break;
            default:
                _logger.LogError("Can't recognize the input type! Please try to specify the input type manually and try again.");
                break;
        }

        if (stroutput == null || msg == null)
        {
            _logger.LogWarning("Serialization/Deserialization probably failed!");
        }
        else
        {
            await Tools.SetClipBoardAsync(stroutput);

            var unksize = Tools.GetUnknownFieldsSize(msg, prototype);
            if (unksize != 0)
            {
                _logger.LogWarning("Message has unknown fields that aren't defined " +
                    "in your proto: {unkSize}/{totalSize} bytes. " +
                    "Please go to protobuf decode-raw tools for more information.",
                    unksize, msg.CalculateSize());
            }
        }
    }

    #region Protobuf Operations
    public static Type FindProtoMessageType(string protoname)
    {
        var conf = Config.Global.EasyProtobufProgram;
        Type? prototype = null;
        var protoNamespace = conf?.ProtoRootNamespace;
        if (protoNamespace == null) prototype = Type.GetType(protoname);
        else prototype = Type.GetType($"{protoNamespace}.{protoname}");
        if (prototype == null) throw new ArgumentException("Find Proto (by name) failed.", nameof(protoname));
        return prototype;
    }

    public static IMessage? Serialize(string protoname, string json)
    {
        return ProtoSerialize.Serialize(FindProtoMessageType(protoname), json);
    }

    public static IMessage? Deserialize(string protoname, byte[] contentbin)
    {
        return ProtoSerialize.Deserialize(FindProtoMessageType(protoname), contentbin);
    }
    #endregion
}
