using System.Reflection;
using CommandLine;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using YYHEggEgg.EasyProtobuf.Configuration;
using YYHEggEgg.EasyProtobuf.Util;
using YYHEggEgg.Logger;
using YYHEggEgg.Shell;
using YYHEggEgg.Shell.Attributes;
using YYHEggEgg.Shell.AutoCompletion;

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
    public override string Description => "Basic Protobuf handler.";

    public override async Task<bool> HandleAsync(ProtobufOption opt, CancellationToken cancellationToken)
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
            return false;
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
                    return false;
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
                    return false;
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
        return true;
    }

    #region Auto fill Protobuf names
    private List<string>? _protoNames;
    private static IEnumerable<string> MatchByName(IEnumerable<string> strings, string text, int index)
    {
        string start = text.Substring(0, index);
        string end = text.Substring(index, text.Length - index);
        return strings.Where((string str) => str.StartsWith(start) && str.EndsWith(end));
    }

    public override SuggestionResult GetSuggestions(string text, int index)
    {
        if (text.Contains(' '))
        {
            return new SuggestionResult();
        }

        if (_protoNames == null)
        {
            var conf = Config.Global.EasyProtobufProgram;
            var protoNamespace = conf?.ProtoRootNamespace;
            _protoNames = (from type in Assembly.GetExecutingAssembly().GetTypes()
                           where type.IsAssignableTo(typeof(IMessage))
                           where type.FullName != null && (protoNamespace == null || type.FullName.StartsWith(protoNamespace) == true)
                           select type.FullName![(protoNamespace == null ? 0 : protoNamespace.Length + 1)..]).ToList();
        }
        IEnumerable<string> enumerable = MatchByName(_protoNames, text, index);
        return new SuggestionResult
        {
            Suggestions = enumerable.ToList()
        };
    }
    #endregion

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
