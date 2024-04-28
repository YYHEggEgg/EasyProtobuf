using YYHEggEgg.EasyProtobuf.resLoader;
using YYHEggEgg.Logger;

namespace YYHEggEgg.EasyProtobuf.Commands.Dispatch;

internal class CurrRegionCmdsAutoCompleteHandler(bool requireSPub) : IAutoCompleteHandler
{
    /// <summary>
    /// Fill out the key_id.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public SuggestionResult GetSuggestions(string text, int index)
    {
        if (text.Trim().Contains(' ')) return new();
        if (index <= text.TrimEnd().Length) return new();
        
        var keyIds = Resources.CPri.Keys
            .Intersect(Resources.LocalSPri.Keys);
        if (requireSPub) keyIds = keyIds.Intersect(Resources.OfficialSPub.Keys);
        return new()
        {
            Suggestions = keyIds.Select(x => x.ToString()).ToList(),
            StartIndex = index,
            EndIndex = -1,
        };
    }
}
