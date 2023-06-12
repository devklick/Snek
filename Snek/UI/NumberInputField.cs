namespace Snek.UI;

public class NumberInputField
{
    public TextBox Label { get; }
    public TextBox InputField { get; }

    public NumberInputField(Position offset, Alignment align, ConsoleColor backgroundColor, ConsoleColor foregroundColor, string? label = null, int? defaultValue = null)
    {
        Label = new TextBox(offset, align, backgroundColor, foregroundColor, label);
        InputField = new TextBox(new Position(offset.X, offset.Y + 1), align, backgroundColor, foregroundColor, null, defaultValue?.ToString());
    }
}