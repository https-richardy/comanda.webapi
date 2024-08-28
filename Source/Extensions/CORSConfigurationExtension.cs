namespace Comanda.WebApi.Extensions;

[ExcludeFromCodeCoverage]
internal static class CORSConfigurationExtension
{
    public static void ConfigureCORS(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
            });

            options.AddPolicy("RestrictedHubPolicy", builder =>
            {
                builder.WithOrigins("http://127.0.0.1:5500");
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowCredentials();
            });
        });
    }
}