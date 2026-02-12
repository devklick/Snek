using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace Snek.Core.Cli;

/// <summary>
/// Defines an argument accepted by a command line interface application.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class CliArgAttribute : Attribute
{
    /// <summary>
    /// The full name of the command line argument
    /// </summary>
    /// <example>--width</example>
    public string FullName { get; }
    /// <summary>
    /// The short name of the command line argument, usually a single character.
    /// </summary>
    /// <example>-w</example>
    public string ShortName { get; }

    /// <summary>
    /// Constructs a <see cref="CliArgAttribute"/> with defined argument names.
    /// </summary>
    /// <param name="fullName">The full name for the arg, e.g. <c>--width</c>. Will be prepended with <c>--</c> if not present.</param>
    /// <param name="shortName">The short name for the arg. Usually one character, e.g. <c>-w</c>. Will be prepended with <c>-</c> if not present.</param>
    public CliArgAttribute(string fullName, string shortName)
    {
        FullName = fullName;
        ShortName = shortName;

        if (!FullName.StartsWith("--")) FullName = $"--{FullName}";
        if (!ShortName.StartsWith('-')) ShortName = $"-{ShortName}";
    }

    /// <summary>
    /// Verifies that the <see cref="value"/> is allowed based on the <see cref="propertyInfo"/>
    /// and certain attributes that may be defines on the property.
    /// </summary>
    /// <param name="propertyInfo">The property that the <see cref="CliArgAttribute"/> is defined on. 
    /// Also the property that the value is intended to be assigned to.</param>
    /// <param name="value">The raw value passed in as a CLI arg.</param>
    /// <returns>An object containing information about the validation result</returns>
    public virtual CliArgValidation Validate(PropertyInfo propertyInfo, object? value)
    {
        var type = propertyInfo.PropertyType;
        if (type == typeof(bool)) return ValidateBoolean(propertyInfo, value);
        if (type == typeof(int)) return ValidateNumber(propertyInfo, value);
        if (type.IsEnum) return ValidateEnum(propertyInfo, value);
        throw new NotImplementedException($"Unsupported type ${type} for CliArgAttribute");
    }

    /// <summary>
    /// Validates a value that is intended to be assigned to a boolean property.
    /// </summary>
    /// <param name="_">The info for the target property</param>
    /// <param name="value">The value intended for the property</param>
    /// <returns>An object containing information about the validation result</returns>
    private static CliArgValidation ValidateBoolean(PropertyInfo _, object? value)
    {
        if (!bool.TryParse((string)value!, out var boolValue))
        {
            return CliArgValidation.Failed();
        }
        return CliArgValidation.Success(boolValue);
    }

    /// <summary>
    /// Validates a value that is intended to be assigned to an integer property.
    /// </summary>
    /// <param name="_">The info for the target property</param>
    /// <param name="value">The value intended for the property</param>
    /// <returns>An object containing information about the validation result</returns>
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

    /// <summary>
    /// Validates a value that is intended to be assigned to an enum property.
    /// </summary>
    /// <param name="_">The info for the target property</param>
    /// <param name="value">The value intended for the property</param>
    /// <returns>An object containing information about the validation result</returns>
    private static CliArgValidation ValidateEnum(PropertyInfo propertyInfo, object? value)
    {
        if (!Enum.TryParse(propertyInfo.PropertyType, (string?)value, out var enumValue))
        {
            return CliArgValidation.Failed();
        }
        return CliArgValidation.Success(enumValue);
    }
}