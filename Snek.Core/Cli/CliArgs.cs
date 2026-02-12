using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Snek.Core.Extensions;
using Snek.Core.Settings;

namespace Snek.Core.Cli;

public class CliArgs
{
    public GameSettings GameSettings { get; } = GameSettings.Default;
    public bool RequiresHelp { get; private set; }
    public CliHelpInfo HelpInfo { get; private set; }
    public List<string> Errors { get; } = [];
    public bool HasError => !Errors.IsEmpty;

    public CliArgs(string[] args)
    {
        var argProps = GetArgProps();

        HelpInfo = GetHelpInfo(argProps);

        if (args.Any(a => a == CliHelpInfo.FullName || a == CliHelpInfo.ShortName))
        {
            RequiresHelp = true;
            return;
        }

        for (int i = 0; i < args.Length; i += 2)
        {
            var name = args[i];
            var value = args.Length >= i + 2 ? args[i + 1] : null;

            var (argProp, attr) = argProps.FirstOrDefault(a => a.attr.FullName == name || a.attr.ShortNames.Any(shortName => shortName == name));

            if (attr == null)
            {
                Errors.Add($"Unknown argument: {name}");
                return;
            }

            var validation = attr.Validate(argProp, value);

            if (!validation.Valid)
            {
                Errors.Add($"Invalid value for argument {name}: {value}");
                return;
            }

            argProp.SetValue(GameSettings, validation.Value);
        }
    }

    private CliHelpInfo GetHelpInfo(IEnumerable<(PropertyInfo argProp, CliArgAttribute attr)> argProps)
    {
        var helpInfo = new CliHelpInfo();
        foreach (var (argProp, attr) in argProps)
        {
            var description = GetDescription(argProp);
            var typeInfo = GetTypeInfo(argProp);

            var cliArg = new CliArgHelpInfo(attr.FullName, attr.ShortNames, description)
            {
                Default = GetDefault(argProp),
                Type = typeInfo.Type,
                Validation = typeInfo.Validation,
                AllowedValues = typeInfo.AllowedValues
            };

            helpInfo.Add(cliArg);
        }

        return helpInfo;
    }

    private string? GetDefault(PropertyInfo prop)
        => prop.GetValue(GameSettings)?.ToString();

    private static string GetDescription(MemberInfo prop)
        => prop.GetCustomAttribute<DescriptionAttribute>()?.Description
        ?? throw new InvalidDataException("A description is required for all CLI args");

    private static CliArgTypeDefinition GetTypeInfo(PropertyInfo argProp)
    {
        if (argProp.PropertyType.IsEnum)
        {
            return GetEnumTypeInfo(argProp);
        }
        else if (argProp.PropertyType == typeof(int))
        {
            return GetNumberTypeInfo(argProp);
        }
        else if (argProp.PropertyType == typeof(bool))
        {
            return GetBoolTypeInfo(argProp);
        }
        throw new NotImplementedException($"Unsupported type ${argProp.PropertyType} for CliArgAttribute");
    }

    private static CliArgTypeDefinition GetBoolTypeInfo(PropertyInfo _)
        => new("Boolean", "True, False");

    private static CliArgTypeDefinition GetNumberTypeInfo(PropertyInfo argProp)
    {
        var type = "Number";
        var validation = "";

        var range = argProp.GetCustomAttribute<RangeAttribute>();
        if (range != null)
        {
            validation = $"min {range.Minimum}, max {range.Maximum}";
        }
        return new(type, validation);
    }

    [UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2026:RequiresUnreferencedCode",
        Justification = "Seems to work, so it's not a problem until it's a problem")]
    [UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2075:RequiresUnreferencedCode",
        Justification = "Seems to work, so it's not a problem until it's a problem")]
    private static CliArgTypeDefinition GetEnumTypeInfo(PropertyInfo argProp)
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
        return new("Enum", $"{string.Join(", ", names)}", allowedValues);
    }

    private static IEnumerable<(PropertyInfo argProp, CliArgAttribute attr)> GetArgProps()
    {
        var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty;

        return typeof(GameSettings).GetProperties(flags)
            .Select(argProp => (argProp, attr: argProp.GetCustomAttribute<CliArgAttribute>()))
            .Where(value => value.attr != null)!;
    }
}