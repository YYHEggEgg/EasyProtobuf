using YYHEggEgg.EasyProtobuf.Configuration;
using YYHEggEgg.Logger;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal static class CommandHistory
{
    public const string HistoryFileName = ".bash_history";
    private static StreamWriter? historyWriter;
    public static string GetHistoryFile()
    {
        var version = EasyProtobufProgram.protobuf_version;
        // tbh i don't want to add a handling case for this,
        // but cuz u can't create a file ends with '.' on
        // Windows, that will lead to an implicit behaviour
        // change. Better not.
        return $"{HistoryFileName}{(string.IsNullOrEmpty(version) ? "" : $".{version}")}";
    }

    public static IEnumerable<string> ReadSavedHistory()
    {
        var file = GetHistoryFile();
        if (File.Exists(file)) return File.ReadAllLines(file);
        else return Array.Empty<string>();
    }

    public static void PushExecuted(string command)
    {
        if (!Config.Global.RestoreHistoryOnRestart) return;
        if (historyWriter == null)
        {
            var version = EasyProtobufProgram.protobuf_version;
            var file = GetHistoryFile();
            historyWriter = new(file, true);
            historyWriter.AutoFlush = true;
        }

        if (command.Length > ConsoleWrapper.HistoryMaximumChars) return;
        historyWriter.WriteLine(command);
    }
}

