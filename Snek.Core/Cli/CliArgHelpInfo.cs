using System.Text;
using Snek.Core.Extensions;

namespace Snek.Core.Cli;

public class CliArgHelpInfo(string fullName, string shortName, string description)
{
    private readonly string _ = "  ";
    public string FullName { get; } = fullName;
    public string ShortName { get; } = shortName;
    public string Description { get; } = description;
    public string? Type { get; set; }
    public string? Validation { get; set; }
    public List<(string Value, string Description)> AllowedValues { get; set; } = new();
    public string? Default { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{_}{FullName}, {ShortName}");
        sb.AppendLine($"{_}{_}{Description}");

        AddTypeAndValidation(sb);
        AddAllowedValues(sb);
        AddDefaultValue(sb);

        return sb.ToString();
    }

    private void AddTypeAndValidation(StringBuilder sb)
    {
        if (Type != null)
        {
            sb.Append($"{_}{_}Type: {Type}");
            if (Validation != null)
            {
                sb.Append($" ({Validation})");
            }
            sb.AppendLine();
        }
    }

    private void AddAllowedValues(StringBuilder sb)
    {
        if (!AllowedValues.IsEmpty)
        {
            foreach (var (value, description) in AllowedValues)
            {
                sb.Append($"{_}{_}{_}{value}");
                if (description != null)
                {
                    sb.Append($": {description}");
                }
                sb.AppendLine();
            }
        }
    }

    private void AddDefaultValue(StringBuilder sb)
    {
        if (!string.IsNullOrEmpty(Default))
        {
            sb.AppendLine($"{_}{_}Default: {Default}");
        }
    }
}