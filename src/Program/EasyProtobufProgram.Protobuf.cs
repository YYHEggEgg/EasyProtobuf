using CommandLine;
using Google.Protobuf;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using YYHEggEgg.EasyProtobuf.Commands;
using YYHEggEgg.EasyProtobuf.Configuration;
using YYHEggEgg.EasyProtobuf.resLoader;
using YYHEggEgg.EasyProtobuf.Util;
using YYHEggEgg.Logger;

namespace YYHEggEgg.EasyProtobuf;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
class ProtobufOption
{
    [Value(0, Required = true)]
    public string Protoname { get; set; }
}
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

internal partial class EasyProtobufProgram
{
    public override string CommandName => nameof(EasyProtobufProgram);
    public override string Usage => throw new NotImplementedException();
    public override string Description => $"(Default) Type proto name and do operations.";

    public override async Task HandleAsync(ProtobufOption opt)
    {
        var conf = Config.Global.EasyProtobufProgram;
        var protoname = opt.Protoname;
        Type? prototype = null;
        try
        {
            prototype = FindProtoMessageType(protoname);
        }
        catch (Exception ex)
        {
            _logger.LogErroTrace(ex, $"Find Proto (by name) failed.");
        }
        if (prototype == null)
        {
            _logger.LogErro($"Proto type or command not found: '{protoname}'.");
            return;
        }
        _logger.LogInfo("Well done! The proto exists.");

        _logger.LogInfo("Please type base64 encoded or HEX protobuf bin data (auto detect):");
        _logger.LogInfo("You can also paste json data to get its serialized data.");

        _logger.LogInfo(EasyInput.MultipleInputNotice);
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
                    _logger.LogInfo($"Deserialized message = null");
                    _logger.LogWarn("Serialization/Deserialization probably failed!");
                    return;
                }
                stroutput = JsonFormatter.Default.Format(msg);
                _logger.LogInfo($"Converted Json:{Environment.NewLine}{stroutput}");
                break;
            case EasyInputType.Json:
                msg = ProtoSerialize.Serialize(prototype, res.ProcessedString ?? string.Empty);

                if (msg == null)
                {
                    _logger.LogInfo($"Serialized message = null");
                    _logger.LogWarn("Serialization/Deserialization probably failed!");
                    return;
                }
                stroutput = Convert.ToBase64String(msg.ToByteArray());
                _logger.LogInfo($"Serialized Base64:{Environment.NewLine}{stroutput}");
                break;
            default:
                _logger.LogErro($"Can't recognize the input type! Please try to specify the input type manually and try again.");
                break;
        }

        if (stroutput == null || msg == null)
        {
            _logger.LogWarn("Serialization/Deserialization probably failed!");
        }
        else
        {
            await Tools.SetClipBoardAsync(stroutput);

            var unksize = Tools.GetUnknownFieldsSize(msg, prototype);
            if (unksize != 0)
            {
                _logger.LogWarn($"Message has unknown fields that aren't defined in your proto: {unksize}/{msg.CalculateSize()} bytes. Please go to protobuf decode-raw tools for more information.");
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
