using CommandLine;
using System.Reflection;
using YYHEggEgg.EasyProtobuf.Commands.AutoCompletion;
using YYHEggEgg.Logger;

namespace YYHEggEgg.EasyProtobuf.Commands;

internal abstract class HasSubCommandsHandlerBase : CommandHandlerBase
{
    private Dictionary<string, OptionsAutoCompleteHandler> _autoCmplHandlersMap;
    private Dictionary<string, string> _subCommandAliasesMap;
    public HasSubCommandsHandlerBase(IEnumerable<Type> optionTypes)
    {
        _autoCmplHandlersMap = [];
        _subCommandAliasesMap = [];
        InitializeSubCommands(optionTypes);
    }

    protected void InitializeSubCommands(IEnumerable<Type> optionTypes)
    {
        foreach (var optionType in optionTypes)
        {
            var verbAttr = optionType.GetCustomAttribute<VerbAttribute>();
            if (verbAttr == null)
                throw new ArgumentException("Provided an option type that doesn't define VerbAttribute.", nameof(optionTypes));

            var subCommandName = verbAttr.Name;
            var aliases = verbAttr.Aliases.ToList();
            aliases.Add(subCommandName);
            if (verbAttr.IsDefault)
                aliases.Add(string.Empty);
            foreach (var alias in aliases)
                _subCommandAliasesMap.Add(alias, subCommandName);

            _autoCmplHandlersMap.Add(subCommandName, new OptionsAutoCompleteHandler(optionType));
        }
    }

    public override SuggestionResult GetSuggestions(string text, int index)
    {
        var args = CommandHandlerBase.ParseAsArgs(text);
        if (index <= args[0].Length) return new();
        var subCommand = args.Count > 1 ? args[1] : string.Empty;

        if (args.Count > 2)
        {
            if (!_subCommandAliasesMap.TryGetValue(subCommand, out var subCommandDef) && !_subCommandAliasesMap.TryGetValue(string.Empty, out subCommandDef))
                return new();
            var subCommandHandler = _autoCmplHandlersMap[subCommandDef];
            return subCommandHandler.GetSuggestions(text, index);
        }

        // Fill out subcommand
        var subStartLimit = text[(args[0].Length + 1)..index];
        var subEndLimit = text[index..];
        return new SuggestionResult
        {
            Suggestions = (from subCommandName in _autoCmplHandlersMap.Keys
                           where subCommandName.StartsWith(subStartLimit) && subCommandName.EndsWith(subEndLimit)
                           select subCommandName).ToList(),
            StartIndex = args[0].Length + 1,
            EndIndex = -1,
        };
    }
}
