namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Responses;

public abstract record AssignAccountantResponse
{
    private AssignAccountantResponse() { }

    public sealed record Success() : AssignAccountantResponse;

    public sealed record Failure(string ErrorMessage) : AssignAccountantResponse;
}