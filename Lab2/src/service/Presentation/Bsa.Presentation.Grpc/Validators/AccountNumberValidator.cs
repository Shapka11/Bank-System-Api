using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Bsa.Presentation.Grpc.Validators;

public static class AccountNumberValidator
{
    public static IEnumerable<ValidationResult> Validate(
        string accountNumber,
        [CallerArgumentExpression(nameof(accountNumber))]
        string? propertyName = null)
    {
        if (string.IsNullOrEmpty(accountNumber))
            yield return new ValidationResult("Account number can't be empty", [propertyName ?? string.Empty]);
    }
}