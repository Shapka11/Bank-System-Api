using Bsa.Presentation.Grpc.Controllers;

namespace Bsa.Presentation.Grpc.Extensions;

public static class WebApplicationExtensions
{
    public static void UsePresentationGrpc(this WebApplication app)
    {
        app.MapGrpcService<AccountController>();
        app.MapGrpcService<InvoiceController>();
        app.MapGrpcService<HistoryOperationController>();
        app.MapGrpcService<AdminController>();
        app.MapGrpcReflectionService();
    }
}
