using System.Diagnostics;
using YYHEggEgg.Logger;

namespace YYHEggEgg.EasyProtobuf.Commands.AutoCompletion;

public class FilePathAutoCompleteHandler : IAutoCompleteHandler
{
    public string CurrentPath { get; set; } = Environment.CurrentDirectory;

    private static char[] GetPathSeparators()
    {
        if (OperatingSystem.IsWindows()) return ['\\', '/'];
        else return ['/'];
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
