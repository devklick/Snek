namespace Snek;

public class TextBox
{
    public ConsoleColor BackgroundColor { get; }
    public ConsoleColor ForegroundColor { get; }
    public string Content => $"{_label}: {_value}";
    /// <summary>
    /// The position relative to the parent object that this text box will be drawn.
    /// </summary>
    /// <remarks>
    /// Note that the <see cref="Position.X"/> position is relative to the <see cref="Align"/>; 
    /// when it's <see cref="Alignment.Left"/>, its relative to the left edge, and when it's 
    /// <see cref="Alignment.Right"/>, it's relative to the right edge.
    /// Otherwise, when it's <see cref="Alignment.Centre"/>, the <see cref="Position.X"/> property will be ignored.
    /// </remarks>
    public Position Anchor { get; }
    public Alignment Align { get; }
    private string? _value;
    private readonly string _label;

    public TextBox(Position anchor, Alignment align, ConsoleColor backgroundColor, ConsoleColor foregroundColor, string label, string? value = null)
    {
        Anchor = anchor;
        BackgroundColor = backgroundColor;
        ForegroundColor = foregroundColor;
        Align = align;
        _label = label;
        _value = value;
    }

    public void SetValue(string value)
    {
        _value = value;
    }
}