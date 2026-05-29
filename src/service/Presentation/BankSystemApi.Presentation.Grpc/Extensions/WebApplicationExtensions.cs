using BankSystemApi.Presentation.Grpc.Controllers;

namespace BankSystemApi.Presentation.Grpc.Extensions;

public static class WebApplicationExtensions
{
    public static void UsePresentationGrpc(this WebApplication app)
    {
        app.MapGrpcService<AccountController>();
        app.MapGrpcService<InvoiceController>();
        app.MapGrpcService<HistoryOperationController>();
        app.MapGrpcService<UserController>();
        app.MapGrpcReflectionService();
    }
}
