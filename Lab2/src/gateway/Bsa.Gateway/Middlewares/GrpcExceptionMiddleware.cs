using Grpc.Core;
using System.Net;

namespace Bsa.Gateway.Middlewares;

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
            else if (e.StatusCode == StatusCode.DeadlineExceeded)
            {
                httpStatusCode = HttpStatusCode.RequestTimeout;
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
            else if (e.StatusCode == StatusCode.ResourceExhausted)
            {
                httpStatusCode = HttpStatusCode.TooManyRequests;
            }
            else if (e.StatusCode is StatusCode.FailedPrecondition or StatusCode.OutOfRange)
            {
                httpStatusCode = HttpStatusCode.BadRequest;
            }
            else if (e.StatusCode == StatusCode.Unimplemented)
            {
                httpStatusCode = HttpStatusCode.NotImplemented;
            }
            else if (e.StatusCode == StatusCode.Internal)
            {
                httpStatusCode = HttpStatusCode.InternalServerError;
            }
            else if (e.StatusCode == StatusCode.Unavailable)
            {
                httpStatusCode = HttpStatusCode.BadGateway;
            }
            else if (e.StatusCode == StatusCode.Cancelled)
            {
                httpStatusCode = HttpStatusCode.RequestTimeout;
            }
            else if (e.StatusCode == StatusCode.Unknown)
            {
                httpStatusCode = HttpStatusCode.InternalServerError;
            }
            else if (e.StatusCode == StatusCode.Aborted)
            {
                httpStatusCode = HttpStatusCode.Conflict;
            }
            else if (e.StatusCode == StatusCode.DataLoss)
            {
                httpStatusCode = HttpStatusCode.ServiceUnavailable;
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