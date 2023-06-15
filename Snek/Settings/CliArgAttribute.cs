using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace Snek.Settings;

[AttributeUsage(AttributeTargets.Property)]
public class CliArgAttribute : Attribute
{
    public string FullName { get; }
    public string ShortName { get; }
    public CliArgAttribute(string fullName, string shortName)
    {
        FullName = fullName;
        ShortName = shortName;

        if (!FullName.StartsWith("--")) FullName = $"--{FullName}";
        if (!ShortName.StartsWith("-")) ShortName = $"-{ShortName}";
    }

    public virtual CliArgValidation Validate(PropertyInfo propertyInfo, object? value)
    {
        var type = propertyInfo.PropertyType;
        if (type == typeof(bool)) return ValidateBoolean(propertyInfo, value);
        if (type == typeof(int)) return ValidateNumber(propertyInfo, value);
        if (type.IsEnum) return ValidateEnum(propertyInfo, value);
        throw new NotImplementedException($"Unsupported type ${type} for CliArgAttribute");
    }

    private static CliArgValidation ValidateBoolean(PropertyInfo _, object? value)
    {
        if (!bool.TryParse((string)value!, out var boolValue))
        {
            return CliArgValidation.Failed();
        }
        return CliArgValidation.Success(boolValue);
    }

    private static CliArgValidation ValidateNumber(PropertyInfo propertyInfo, object? value)
    {
        var range = propertyInfo.GetCustomAttribute<RangeAttribute>();

        int? number = null;

        if (value is int intVal)
        {
            number = intVal;
        }
        else if (value is string strVal)
        {
            if (int.TryParse(strVal, out var n)) number = n;
        }

        if (number == null)
        {
            return CliArgValidation.Failed();
        }
        if (range != null && !range.IsValid(number))
        {
            return CliArgValidation.Failed();
        }

        return CliArgValidation.Success(number);
    }

    private static CliArgValidation ValidateEnum(PropertyInfo propertyInfo, object? value)
    {
        if (!Enum.TryParse(propertyInfo.PropertyType, (string?)value, out var enumValue))
        {
            return CliArgValidation.Failed();
        }
        return CliArgValidation.Success(enumValue);
    }
}