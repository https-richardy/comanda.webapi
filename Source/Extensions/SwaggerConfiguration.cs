namespace Comanda.WebApi.Extensions;

public static class SwaggerConfigurationExtension
{
    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Comanda Web API",
                Version = "v1",
                Description = "Comanda Web API documentation",
            });
        });
    }
}