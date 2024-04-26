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

    private static char[] GetPathSeparators()
    {
        if (OperatingSystem.IsWindows()) return [ '\\', '/' ];
        else return [ '/' ];
    }

    public SuggestionResult GetSuggestions(string text, int index)
    {
        var left = text[..index];
        var right = text[index..];
        string endlimit;
        if (right.Contains('"')) endlimit = right[..right.IndexOf('"')];
        else endlimit = right;

        // unclosed " trigger the file-name completions
        var leftCount = left.Count(c => c == '"');
        // Log.Info($"left: '{left}', right: '{right}', endlimit: '{endlimit}', leftCount: '{leftCount}'");
        if (leftCount % 2 != 1) return new();
        if (endlimit.IndexOfAny(GetPathSeparators()) >= 0) return new();

        var startIndex = left.LastIndexOf('"') + 1;
        Debug.Assert(startIndex > 0);
        var endIndex = text.IndexOf('"', index);
        // Log.Info($"index: [{startIndex}, {endIndex})");

        var requestedPath = left[startIndex..];
        var separatorIdx = requestedPath.LastIndexOfAny(GetPathSeparators());
        // Log.Info($"requestedPath: '{requestedPath}', separatorIdx: {separatorIdx}");
        var inputDir = separatorIdx == -1 ? requestedPath : requestedPath[..separatorIdx];
        if (inputDir == string.Empty) inputDir = ".";

        var parentDir = Path.GetFullPath(inputDir, CurrentPath);
        var startlimit = requestedPath[(separatorIdx + 1)..];
        // Log.Info($"parentDir: '{parentDir}'', startlimit: {startlimit}");

        var enumeratedNames = Directory.EnumerateDirectories(parentDir, "*", SearchOption.TopDirectoryOnly)
            .Concat(Directory.EnumerateFiles(parentDir, "*.*", SearchOption.TopDirectoryOnly))
            .Select(x => Path.GetFileName(x));
        var names = from name in enumeratedNames
                    where name.StartsWith(startlimit) && name.EndsWith(endlimit)
                    select $"{inputDir}{Path.DirectorySeparatorChar}{name}";
        // Log.Info($"names: {names}");
        return new()
        {
            Suggestions = names.ToList(),
            StartIndex = startIndex,
            EndIndex = endIndex,
        };
    }
}
