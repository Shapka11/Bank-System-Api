using Grpc.Core;
using Grpc.Core.Interceptors;
using System.ComponentModel.DataAnnotations;

namespace BankSystemApi.Presentation.Grpc.Interceptors;

public sealed class ValidationInterceptor : Interceptor
{
    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        if (request is IValidatableObject validatableObject)
        {
            var validationContext = new ValidationContext(validatableObject);
            ValidationResult[] errors = validatableObject.Validate(validationContext).ToArray();

            if (errors is not [])
            {
                string formattedErrors = string.Join(
                    ", ",
                    errors.Select(error => $"'{error.ErrorMessage}'"));

                string errorMessage = $"""
                                       Validation failed with errors:
                                           {formattedErrors}
                                       """;

                throw new RpcException(new Status(StatusCode.FailedPrecondition, errorMessage));
            }
        }

        return continuation(request, context);
    }
}
