namespace BankSystemApi.Gateway.Presentation.Http.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UsePresentationHttp(this WebApplication application)
    {
        application.MapControllers();
        return application;
    }
}
