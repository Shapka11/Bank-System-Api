using Grpc.Core;
using System.Net;

namespace BankSystemApi.Gateway.Middlewares;

public sealed class GrpcExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (RpcException e)
        {
            HttpStatusCode httpStatusCode;
            if (e.StatusCode == StatusCode.OK)
            {
                httpStatusCode = HttpStatusCode.OK;
            }
            else if (e.StatusCode == StatusCode.InvalidArgument)
            {
                httpStatusCode = HttpStatusCode.BadRequest;
            }
            else if (e.StatusCode == StatusCode.NotFound)
            {
                httpStatusCode = HttpStatusCode.NotFound;
            }
            else if (e.StatusCode == StatusCode.AlreadyExists)
            {
                httpStatusCode = HttpStatusCode.Conflict;
            }
            else if (e.StatusCode == StatusCode.PermissionDenied)
            {
                httpStatusCode = HttpStatusCode.Forbidden;
            }
            else if (e.StatusCode == StatusCode.Unauthenticated)
            {
                httpStatusCode = HttpStatusCode.Unauthorized;
            }
            else if (e.StatusCode == StatusCode.Internal)
            {
                httpStatusCode = HttpStatusCode.InternalServerError;
            }
            else if (e.StatusCode == StatusCode.FailedPrecondition)
            {
                httpStatusCode = HttpStatusCode.BadRequest;
            }
            else if (e.StatusCode == StatusCode.OutOfRange)
            {
                httpStatusCode = HttpStatusCode.BadRequest;
            }
            else
            {
                httpStatusCode = HttpStatusCode.InternalServerError;
            }

            context.Response.StatusCode = (int)httpStatusCode;
            await context.Response.WriteAsJsonAsync(new { Message = e.Status.Detail });
        }
    }
}