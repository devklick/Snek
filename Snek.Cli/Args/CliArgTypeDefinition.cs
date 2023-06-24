namespace Snek.Cli.Args;

public class CliArgTypeDefinition
{
    public string Type { get; set; }
    public string Validation { get; set; }
    public List<(string name, string description)> AllowedValues { get; set; }
    public CliArgTypeDefinition(string type, string validation, List<(string name, string description)>? allowedValues = null)
    {
        Type = type;
        Validation = validation;
        AllowedValues = allowedValues ?? new();
    }
}