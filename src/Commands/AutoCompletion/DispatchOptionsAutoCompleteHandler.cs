using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYHEggEgg.EasyProtobuf.Commands;
using YYHEggEgg.Logger;

namespace YYHEggEgg.EasyProtobuf.src.Commands.AutoCompletion;

internal class DispatchOptionsAutoCompleteHandler : IAutoCompleteHandler
{
    private List<CommandHandlerBase> _handlers;

    public DispatchOptionsAutoCompleteHandler(List<CommandHandlerBase> handlers)
    {
        _handlers = handlers;
    }

    public SuggestionResult GetSuggestions(string text, int index)
    {
        var separaorIdx = text.IndexOf(' ');
        if (separaorIdx < 0)
            throw new NotImplementedException();
        var commandName = text[..separaorIdx];
        var handler = _handlers.Where(x => x.CommandName == commandName).FirstOrDefault();
        if (handler == null) return new();
        else return handler.GetSuggestions(text, index);
    }
}
