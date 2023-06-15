using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Snek.Settings;

public class CliArgs
{
    public GameSettings GameSettings { get; } = GameSettings.Default;
    public bool Help { get; private set; }
    public List<string> HelpInfo { get; private set; }
    public bool Error { get; }
    public CliArgs(string[] args)
    {
        var argProps = GetArgProps();

        HelpInfo = GetHelpInfo(argProps);

        if (args.Any(a => a == "--help" || a == "-h"))
        {
            Help = true;
            return;
        }

        for (int i = 0; i < args.Length; i += 2)
        {
            var name = args[i];
            var value = args.Length >= i + 2 ? args[i + 1] : null;

            var (argProp, attr) = argProps.FirstOrDefault(a => a.attr.FullName == name || a.attr.ShortName == name);

            if (attr == null)
            {
                Error = Help = true;
                return;
            }

            var validation = attr.Validate(argProp, value);

            if (!validation.Valid)
            {
                Error = Help = true;
                return;
            }

            argProp.SetValue(GameSettings, validation.Value);
        }
    }

    private static List<string> GetHelpInfo(IEnumerable<(PropertyInfo argProp, CliArgAttribute attr)> argProps)
    {
        var helpInfo = new List<string>() { "Supported arguments:" };

        foreach (var (argProp, attr) in argProps)
        {
            var argNames = $"{attr.FullName}, {attr.ShortName} ";
            var argType = "";
            if (argProp.PropertyType.IsEnum)
            {
                argType = GetEnumTypeInfo(argProp);
            }
            else if (argProp.PropertyType == typeof(int))
            {
                argType = GetNumberTypeInfo(argProp);
            }
            else if (argProp.PropertyType == typeof(bool))
            {
                argType = GetBoolTypeInfo(argProp);
            }
            else
            {
                throw new NotImplementedException($"Unsupported type ${argProp.PropertyType} for CliArgAttribute");
            }

            helpInfo.Add($"\t{argNames}\t{argType}");
        }

        return helpInfo;
    }

    private static string GetBoolTypeInfo(PropertyInfo _)
        => "[boolean, true, false]";

    private static string GetNumberTypeInfo(PropertyInfo argProp)
    {
        var argInfo = $"[number";
        var range = argProp.GetCustomAttribute<RangeAttribute>();
        if (range != null)
        {
            argInfo = $"{argInfo}, min {range.Minimum}, max {range.Maximum}";
        }
        argInfo += "]";
        return argInfo;
    }

    private static string GetEnumTypeInfo(PropertyInfo argProp)
    {
        var values = Enum.GetValues(argProp.PropertyType).Cast<Enum>().Select(s => s.ToString()).ToList();
        return $"[{string.Join(", ", values)}]";
    }

    private static IEnumerable<(PropertyInfo argProp, CliArgAttribute attr)> GetArgProps()
    {
        var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty;

        return typeof(GameSettings).GetProperties(flags)
            .Select(argProp => (argProp, attr: argProp.GetCustomAttribute<CliArgAttribute>()))
            .Where(value => value.attr != null)!;
    }
}