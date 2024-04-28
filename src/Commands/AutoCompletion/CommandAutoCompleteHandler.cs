using Google.Protobuf;
using System.Reflection;
using YYHEggEgg.EasyProtobuf.Commands;
using YYHEggEgg.EasyProtobuf.Configuration;
using YYHEggEgg.Logger;

namespace YYHEggEgg.EasyProtobuf.Commands.AutoCompletion;

internal class CommandAutoCompleteHandler : IAutoCompleteHandler
{
    private IEnumerable<string> _commandNames;
    private IEnumerable<string> _protoNames;

    public CommandAutoCompleteHandler(List<CommandHandlerBase> commands)
    {
        _commandNames = commands.Select(x => x.CommandName);
        var conf = Config.Global.EasyProtobufProgram;
        var protoNamespace = conf?.ProtoRootNamespace;
        _protoNames = (from type in Assembly.GetExecutingAssembly().GetTypes()
                       where type.IsAssignableTo(typeof(IMessage))
                       where type.FullName != null && (protoNamespace == null || type.FullName.StartsWith(protoNamespace) == true)
                       select type.FullName![(protoNamespace == null ? 0 : protoNamespace.Length + 1)..]).ToList();
    }

    private static IEnumerable<string> MatchByName(IEnumerable<string> strings, string text, int index)
    {
        var start = text[..index];
        var end = text[index..];
        return from str in strings
               where str.StartsWith(start) && str.EndsWith(end)
               select str;
    }

    public SuggestionResult GetSuggestions(string text, int index)
    {
        if (text.Contains(' ')) return new();
        var matches = MatchByName(_commandNames, text, index)
            .Concat(MatchByName(_protoNames, text, index));
        return new()
        {
            Suggestions = matches.ToList(),
        };
    }
}
