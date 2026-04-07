using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Bsa.Presentation.Grpc.Validators;

public static class PageSizeValidator
{
    public static IEnumerable<ValidationResult> Validate(
        int pageSize,
        [CallerArgumentExpression(nameof(pageSize))]
        string? propertyName = null)
    {
        const int maxPageSize = 1000;

        if (pageSize > maxPageSize)
        {
            yield return new ValidationResult(
                $"Page size must me least then {maxPageSize} pages.",
                [propertyName ?? string.Empty]);
        }
    }
}