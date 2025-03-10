namespace Comanda.WebApi.Extensions;

[ExcludeFromCodeCoverage]
public static class ServicesExtension
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        MercadoPagoConfig.AccessToken = configuration["MercadoPago:AccessToken"];

        services.AddSignalR()
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                options.PayloadSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        services.ConfigureCORS(configuration);

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                /*
                    The following line is added to prevent issues related to circular references
                    during JSON serialization. It instructs the JsonSerializer to ignore cycles.
                */
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        services.ConfigureSwagger();

        services.AddDataPersistence(configuration);
        services.ConfigureIdentity();

        services.AddMediator();
        services.AddValidation();
        services.AddMapping();

        services.AddJwtBearer(configuration);
        services.AddApplicationServices(configuration);

        services.AddGeminiClient(options =>
        {
            options.ApiKey = configuration["Gemini:ApiKey"]!;
        });
    }
}