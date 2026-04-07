using Bsa.Presentation.Grpc.Validators;
using System.ComponentModel.DataAnnotations;

namespace Bsa.CsharpBackend.Grpc;

public sealed partial class PayInvoiceRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return ValidateCore(validationContext)
            .Concat(GuidValidator.Validate(SessionId));
    }

    public IEnumerable<ValidationResult> ValidateCore(ValidationContext validationContext)
    {
        if (InvoiceId < 0)
            yield return new ValidationResult("Invoice id cannot be negative", [nameof(InvoiceId)]);
    }
}