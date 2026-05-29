using BankSystemApi.Presentation.Grpc.Validators;
using System.ComponentModel.DataAnnotations;

namespace BankSystemApi.Users.Grpc;

public sealed partial class GetUsersRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return AuthorizationIds.SelectMany(id => GuidValidator.Validate(id));
    }
}