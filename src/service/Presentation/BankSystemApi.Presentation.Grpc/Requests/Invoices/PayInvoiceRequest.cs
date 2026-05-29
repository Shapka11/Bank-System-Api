using BankSystemApi.Presentation.Grpc.Validators;
using System.ComponentModel.DataAnnotations;

namespace BankSystemApi.Invoices.Grpc;

public sealed partial class PayInvoiceRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return ValidateCore(validationContext)
            .Concat(GuidValidator.Validate(UserId));
    }

    public IEnumerable<ValidationResult> ValidateCore(ValidationContext validationContext)
    {
        if (InvoiceId < 0)
            yield return new ValidationResult("Invoice id cannot be negative", [nameof(InvoiceId)]);
    }
}