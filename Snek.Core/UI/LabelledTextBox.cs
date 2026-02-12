namespace Snek.Core.UI;

public class LabelledTextBox(LabelPosition labelPosition, Position offset, Alignment align,
    ConsoleColor backgroundColor, ConsoleColor foregroundColor, string label, string? value = null)
    : TextBox(offset, align, backgroundColor, foregroundColor, value)
{
    public TextBox Label { get; set; } = new TextBox(offset, align, backgroundColor, foregroundColor, label);
    public LabelPosition LabelPosition { get; set; } = labelPosition;

    /// <summary>
    /// Gets the lines that make up the label and the text box. 
    /// If <see cref="LabelPosition"/> is <see cref="LabelPosition.Above"/>, two items
    /// will be returned; one of the label and one for the text. Otherwise, 
    /// only one value will be returned, which includes the label and the text.
    /// </summary>
    public override List<string?> GetLines()
    {
        var lines = new List<string?>();
        switch (LabelPosition)
        {
            case LabelPosition.InFront:
                lines.Add($"{Label.Value}: {Value}");
                break;
            case LabelPosition.Above:
                lines.Add(Label.Value);
                lines.Add(Value);
                break;
        }
        return lines;
    }
}