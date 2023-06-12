namespace Snek.UI;

public class OptionSetInputField<TEnum> where TEnum : Enum
{
    public TextBox Label { get; set; }
    public OptionSetOptions<TEnum> Options { get; set; }
    public OptionSetInputField(Position offset, Alignment align, ConsoleColor backgroundColor, ConsoleColor foregroundColor, string? label = null, TEnum? defaultValue = default)
    {
        Label = new TextBox(offset, align, backgroundColor, foregroundColor, label);
        Options = new OptionSetOptions<TEnum>(defaultValue);
    }
}