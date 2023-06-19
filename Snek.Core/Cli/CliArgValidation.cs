namespace Snek.Core.Cli;

public class CliArgValidation
{
    public object? Value { get; }
    public bool Valid { get; }

    public CliArgValidation(bool valid, object? value = null)
    {
        Valid = valid;
        Value = value;
    }

    public static CliArgValidation Failed()
        => new(false);
    public static CliArgValidation Success(object value)
        => new(true, value);
}