namespace Comanda.WebApi.Extensions;

[ExcludeFromCodeCoverage]
internal static class CORSConfigurationExtension
{
    public static void ConfigureCORS(this IServiceCollection services, IConfiguration configuration)
    {
        var clientOrigin = configuration.GetValue<string>("ClientSettings:Address")!;

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
                builder.WithOrigins(clientOrigin);
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowCredentials();
            });
        });
    }
}