using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace BankSystemApi.Presentation.Grpc.Validators;

public static class GuidValidator
{
    public static IEnumerable<ValidationResult> Validate(
        string id,
        [CallerArgumentExpression(nameof(id))]
        string? propertyName = null)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrEmpty(id))
            yield return new ValidationResult("Id can't be empty", [propertyName ?? string.Empty]);

        if (Guid.TryParse(id, out Guid _) is false)
            yield return new ValidationResult("Id is not Guid", [propertyName ?? string.Empty]);
    }
}