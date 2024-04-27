using YYHEggEgg.Logger;

namespace YYHEggEgg.EasyProtobuf.Commands.AutoCompletion;

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