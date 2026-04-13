using System.ComponentModel.DataAnnotations;

namespace Bsa.Gateway.Presentation.Http.Attributes;

public sealed class NotWhiteSpaceAttribute : ValidationAttribute
{
    public NotWhiteSpaceAttribute()
    {
        ErrorMessage = "The field {0} cannot be empty or contain only whitespaces.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string str && string.IsNullOrWhiteSpace(str))
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        return ValidationResult.Success;
    }
}