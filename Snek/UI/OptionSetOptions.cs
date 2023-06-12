namespace Snek.UI;

public class OptionSetOptions<TEnum> where TEnum : Enum
{
    private readonly List<OptionSetOption<TEnum>> _options = new();
    private int _currentIndex;

    public OptionSetOptions(TEnum? defaultValue = default)
    {
        var currentIndex = 0;
        foreach (var value in Enum.GetValues(typeof(TEnum)))
        {
            var enumValue = (TEnum)value;
            var selected = defaultValue == null || enumValue.Equals(defaultValue);
            _options.Add(new OptionSetOption<TEnum>(enumValue, selected));
            if (selected) _currentIndex = currentIndex;
            currentIndex++;
        }
    }

    public TEnum SelectNext()
        => SelectAtIndex(_currentIndex + 1);

    public TEnum SelectPrevious()
        => SelectAtIndex(_currentIndex - 1);

    private TEnum SelectAtIndex(int index)
    {
        if (index >= 0 && index < _options.Count - 1)
        {
            _currentIndex = index;
            _options.ForEach(o => o.DeSelect());
            _options[_currentIndex].Select();
        }
        return _options[_currentIndex].Value;
    }
}