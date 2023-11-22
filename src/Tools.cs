using System.Reflection;
using Google.Protobuf;
using TextCopy;
using YYHEggEgg.Logger;

namespace YYHEggEgg.EasyProtobuf;

internal static class Tools
{
    #region Clipboard (TextCopy)
    public static void SetClipBoard(string text)
    {
        try
        {
            ClipboardService.SetText(text);
            Log.Info("Result copied to clipboard.", nameof(SetClipBoard));
        }
        catch (Exception ex)
        {
            LogTrace.WarnTrace(ex, nameof(SetClipBoard), $"Copy to clipboard failed. ");
        }
    }

    public static async Task SetClipBoardAsync(string text)
    {
        try
        {
            await ClipboardService.SetTextAsync(text);
            Log.Info("Result copied to clipboard.", nameof(SetClipBoard));
        }
        catch (Exception ex)
        {
            LogTrace.WarnTrace(ex, nameof(SetClipBoard), $"Copy to clipboard failed. ");
        }
    }
    #endregion

    #region Protobuf
    public static int GetUnknownFieldsSize(object message, Type prototype)
    {
        var log = Log.GetChannel(nameof(GetUnknownFieldsSize));
        var unkFieldSet_field = prototype.GetField("_unknownFields", BindingFlags.NonPublic | BindingFlags.Instance);
        if (unkFieldSet_field == null)
        {
            log.LogVerb($"Warn: unkFieldSet_field == null");
            return 0;
        }
        var unkFieldSet = (UnknownFieldSet?)unkFieldSet_field.GetValue(message);
        if (unkFieldSet == null)
        {
            log.LogVerb($"Warn: unkFieldSet == null");
            return 0;
        }
        return unkFieldSet.CalculateSize();
    }
    #endregion
}