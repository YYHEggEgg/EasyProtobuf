using Google.Protobuf;
using System.Diagnostics;
using System.Reflection;
using YYHEggEgg.EasyProtobuf.Commands;
using YYHEggEgg.EasyProtobuf.Configuration;
using YYHEggEgg.Logger;

namespace YYHEggEgg.EasyProtobuf;

internal class MultipleAutoCompletionHandler : IAutoCompleteHandler
{
    private List<IAutoCompleteHandler> _handlers = new();
    public void PushComponent(IAutoCompleteHandler autoCompleteHandler) => _handlers.Add(autoCompleteHandler);
    public SuggestionResult GetSuggestions(string text, int index)
    {
        foreach (var handler in _handlers)
        {
            var result = handler.GetSuggestions(text, index);
            if (result.Suggestions?.Any() == true) return result;
        }
        return new();
    }
}

internal class CommandAutoCompleteHandler : IAutoCompleteHandler
{
    private IEnumerable<string> _commandNames;
    private IEnumerable<string> _protoNames;

    public CommandAutoCompleteHandler(List<CommandHandlerBase> commands)
    {
        _commandNames = commands.Select(x => x.CommandName);
        var conf = Config.Global.EasyProtobufProgram;
        var protoNamespace = conf?.ProtoRootNamespace;
        _protoNames = from type in Assembly.GetExecutingAssembly().GetTypes()
                      where type.IsGenericType && type.IsAssignableTo(typeof(IMessage))
                      where type.FullName != null && (protoNamespace == null || type.FullName.StartsWith(protoNamespace) == true)
                      select type.FullName![(protoNamespace?.Length ?? 0)..];
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

internal class FilePathAutoCompleteHandler : IAutoCompleteHandler
{
    public string CurrentPath { get; set; } = Environment.CurrentDirectory;

    public SuggestionResult GetSuggestions(string text, int index)
    {
        var left = text[..index];
        var right = text[index..];

        // unclosed " trigger the file-name completions
        var leftCount = left.Count(c => c == '"');
        if (leftCount % 2 != 1) return new();

        var startIndex = left.LastIndexOf('"') + 1;
        Debug.Assert(startIndex > 0);
        var endIndex = right.IndexOf('"');

        throw new NotImplementedException();
    }
}