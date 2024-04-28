using CommandLine;
using System.Reflection;
using YYHEggEgg.Logger;

namespace YYHEggEgg.EasyProtobuf.Commands.AutoCompletion;

public class OptionsAutoCompleteHandler : IAutoCompleteHandler
{
    private List<string> _autoCmplOptions;
    private Dictionary<string, string> _availableOptions;
    private Type _optType;

    public OptionsAutoCompleteHandler(Type optType)
    {
        _autoCmplOptions = [];
        _availableOptions = [];
        _optType = optType;
        InitializeOptions();
    }

    public SuggestionResult GetSuggestions(string text, int index)
    {
        var args = CommandHandlerBase.ParseAsArgs(text);
        if (index <= args[0].Length)
            throw new NotImplementedException($"Unexpected Internal Error: Should be handled by {nameof(CommandAutoCompleteHandler)}."); // Should not be dispatched here.
        if (text[index - 1] != ' ') return new();

        var startIndex = index;
        var endIndex = text.IndexOf(' ', index);
        if (endIndex == -1) endIndex = text.Length;
        var replaced = text[startIndex..endIndex];
        if (replaced.Contains('"')) return new();

        HashSet<string> excludeOptions = [];
        foreach (var arg in args)
        {
            var defValIndex = arg.LastIndexOf('=');
            if (defValIndex == -1) defValIndex = arg.Length;
            var curArg = arg[..defValIndex];
            foreach (var pair in _availableOptions)
            {
                if (curArg == pair.Key)
                {
                    excludeOptions.Add(pair.Value);
                }
            }
        }

        List<string> suggestions = new(_autoCmplOptions
            .Except(excludeOptions).Where(x => x.StartsWith(replaced)));
        suggestions.Sort();
        return new()
        {
            Suggestions = suggestions,
            StartIndex = startIndex,
            EndIndex = endIndex,
        };
    }

    private void InitializeOptions()
    {
        var properties = _optType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
        foreach (var property in properties)
        {
            var optAttr = property.GetCustomAttribute<OptionAttribute>();
            if (optAttr == null) continue;
            var optName = $"--{optAttr.LongName}";
            if (optName == "--")
                optName = $"-{optAttr.ShortName}";
            _autoCmplOptions.Add(optName);
            if (!string.IsNullOrEmpty(optAttr.ShortName))
                _availableOptions.Add($"-{optAttr.ShortName}", optName);
            if (!string.IsNullOrEmpty(optAttr.LongName))
                _availableOptions.Add($"--{optAttr.LongName}", optName);
        }
    }
}

public class OptionsAutoCompleteHandler<TOptions> : OptionsAutoCompleteHandler
{
    public OptionsAutoCompleteHandler() : base(typeof(TOptions))
    {
    }
}
