using YYHEggEgg.EasyProtobuf.Configuration;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal static class CommandHistory
{
    public const string HistoryFileName = ".bash_history";
    private static StreamWriter? historyWriter;

    public static IEnumerable<string> ReadSavedHistory()
    {
        var version = EasyProtobufProgram.protobuf_version;
        var file = $"{HistoryFileName}.{version}";
        if (File.Exists(file)) return File.ReadAllLines(file);
        else return Array.Empty<string>();
    }

    public static void PushExecuted(string command)
    {
        if (!Config.Global.RestoreHistoryOnRestart) return;
        if (historyWriter == null)
        {
            var version = EasyProtobufProgram.protobuf_version;
            var file = $"{HistoryFileName}.{version}";
            historyWriter = new(file, true);
            historyWriter.AutoFlush = true;
        }

        historyWriter.WriteLine(command);
    }
}

