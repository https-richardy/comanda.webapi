namespace Comanda.WebApi.Extensions;

public static class CORSConfigurationExtension
{
    public static void ConfigureCORS(this IApplicationBuilder app)
    {
        app.UseCors(options =>
        {
            options.AllowAnyHeader();
            options.AllowAnyMethod();
            options.AllowAnyOrigin();
        });
    }
}