namespace Snek.UI;

public class OptionSetOption<TEnum> where TEnum : Enum
{
    public TEnum Value { get; }
    public bool Selected { get; private set; }

    public OptionSetOption(TEnum value, bool selected)
    {
        Value = value;
        Selected = selected;
    }

    public void Select() => Selected = true;
    public void DeSelect() => Selected = false;
}