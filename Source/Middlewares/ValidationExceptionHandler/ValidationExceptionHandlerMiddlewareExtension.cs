namespace Comanda.WebApi.Middlewares;

public static class ValidationExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseValidationExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ValidationExceptionHandlerMiddleware>();
    }
}