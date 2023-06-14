namespace Snek.UI;

public class LabelledNumberInputField : LabelledTextBox
{
    public LabelledNumberInputField(LabelPosition labelPosition, Position offset, Alignment align,
        ConsoleColor backgroundColor, ConsoleColor foregroundColor, string label, int? defaultValue = null)
        : base(labelPosition, offset, align,
         backgroundColor, foregroundColor, label, defaultValue?.ToString())
    {
    }
}