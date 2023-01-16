namespace Snek;

public class TextBox
{
    public ConsoleColor BackgroundColor { get; }
    public ConsoleColor ForegroundColor { get; }
    public string Content => $"{_label}: {_value}";
    public Position Anchor { get; }
    private readonly Alignment _align;
    private string? _value;
    private string? _label;

    public TextBox(Position anchor, Alignment align, ConsoleColor backgroundColor, ConsoleColor foregroundColor, string? label, string? value = null)
    {
        Anchor = anchor;
        BackgroundColor = backgroundColor;
        ForegroundColor = foregroundColor;
        _align = align;
        _label = label;
        _value = value;
    }

    public void UpdateValue(string? value)
    {
        _value = value;
    }

    public void SetValue(string value)
    {
        _value = value;
    }
}