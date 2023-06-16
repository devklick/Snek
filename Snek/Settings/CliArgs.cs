using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Snek.Settings;

public class CliArgs
{
    public GameSettings GameSettings { get; } = GameSettings.Default;
    public bool Help { get; private set; }
    public CliHelpInfo HelpInfo { get; private set; }
    public bool Error { get; }
    public CliArgs(string[] args)
    {
        var argProps = GetArgProps();

        HelpInfo = GetHelpInfo(argProps);

        if (args.Any(a => a == CliHelpInfo.FullName || a == CliHelpInfo.ShortName))
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

    private static CliHelpInfo GetHelpInfo(IEnumerable<(PropertyInfo argProp, CliArgAttribute attr)> argProps)
    {
        var helpInfo = new CliHelpInfo();
        foreach (var (argProp, attr) in argProps)
        {
            var description = GetDescription(argProp);
            var type = "";
            var validation = "";
            var allowedValues = new List<(string, string)>();
            if (argProp.PropertyType.IsEnum)
            {
                (type, validation, allowedValues) = GetEnumTypeInfo(argProp);
            }
            else if (argProp.PropertyType == typeof(int))
            {
                (type, validation) = GetNumberTypeInfo(argProp);
            }
            else if (argProp.PropertyType == typeof(bool))
            {
                (type, validation) = GetBoolTypeInfo(argProp);
            }
            else
            {
                throw new NotImplementedException($"Unsupported type ${argProp.PropertyType} for CliArgAttribute");
            }
            var cliArg = new CliArgHelpInfo(attr.FullName, attr.ShortName, description)
            {
                Default = GetDefault(argProp),
                Type = type,
                Validation = validation,
                AllowedValues = allowedValues
            };


            helpInfo.Add(cliArg);
        }

        return helpInfo;
    }

    private static string? GetDefault(PropertyInfo prop)
    {
        return prop.GetValue(GameSettings.Default)?.ToString();
    }

    private static string GetDescription(MemberInfo prop)
        => prop.GetCustomAttribute<DescriptionAttribute>()?.Description
        ?? throw new InvalidDataException("A description is required for all CLI args");

    private static (string type, string validation) GetBoolTypeInfo(PropertyInfo _)
        => ("Boolean", "True, False");

    private static (string type, string validation) GetNumberTypeInfo(PropertyInfo argProp)
    {
        var type = "Number";
        var validation = "";

        var range = argProp.GetCustomAttribute<RangeAttribute>();
        if (range != null)
        {
            validation = $"min {range.Minimum}, max {range.Maximum}";
        }
        return (type, validation);
    }

    [UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2026:RequiresUnreferencedCode",
        Justification = "Seems to work, so it's not a problem until it's a problem")]
    [UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2075:RequiresUnreferencedCode",
        Justification = "Seems to work, so it's not a problem until it's a problem")]
    private static (string type, string validation, List<(string name, string description)> allowedValues) GetEnumTypeInfo(PropertyInfo argProp)
    {
        var names = new List<string>();
        var allowedValues = Enum.GetValues(argProp.PropertyType).Cast<Enum>().Select(e =>
        {
            var name = e.ToString();
            names.Add(name);
            var members = argProp.PropertyType.GetMember(name);
            var description = GetDescription(members.First());
            return (name, description);
        }).ToList();
        return ("Enum", $"{string.Join(", ", names)}", allowedValues);
    }

    private static IEnumerable<(PropertyInfo argProp, CliArgAttribute attr)> GetArgProps()
    {
        var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty;

        return typeof(GameSettings).GetProperties(flags)
            .Select(argProp => (argProp, attr: argProp.GetCustomAttribute<CliArgAttribute>()))
            .Where(value => value.attr != null)!;
    }
}