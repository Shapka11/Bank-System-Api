using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Bsa.Presentation.Grpc.Validators;

public static class PasswordValidator
{
    public static IEnumerable<ValidationResult> Validate(
        string password,
        [CallerArgumentExpression(nameof(password))]
        string? propertyName = null)
    {
        if (string.IsNullOrEmpty(password))
            yield return new ValidationResult("Password can't be empty", [propertyName ?? string.Empty]);

        if (password.Length < 4)
        {
            yield return new ValidationResult(
                "Password must have at least 4 characters",
                [propertyName ?? string.Empty]);
        }
    }
}
