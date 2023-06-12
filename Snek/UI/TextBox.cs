namespace Snek.UI;

public class TextBox
{
    public ConsoleColor BackgroundColor { get; }
    public ConsoleColor ForegroundColor { get; }
    public string? Content => string.IsNullOrEmpty(Label) ? ValueString : $"{Label}: {ValueString}";
    /// <summary>
    /// The position relative to the parent object that this text box will be drawn.
    /// </summary>
    /// <remarks>
    /// Note that the <see cref="Position.X"/> position is relative to the <see cref="Align"/>; 
    /// when it's <see cref="Alignment.Left"/>, its relative to the left edge, and when it's 
    /// <see cref="Alignment.Right"/>, it's relative to the right edge.
    /// Otherwise, when it's <see cref="Alignment.Centre"/>, the <see cref="Position.X"/> property will be ignored.
    /// </remarks>
    public Position Offset { get; }
    public Alignment Align { get; }
    public string? ValueString { get; set; }
    public string? Label { get; }

    public TextBox(Position offset, Alignment align, ConsoleColor backgroundColor, ConsoleColor foregroundColor, string? label = null, string? value = null)
    {
        Offset = offset;
        BackgroundColor = backgroundColor;
        ForegroundColor = foregroundColor;
        Align = align;
        Label = label;
        ValueString = value;
    }

    public void SetValue(string value)
    {
        ValueString = value;
    }
}