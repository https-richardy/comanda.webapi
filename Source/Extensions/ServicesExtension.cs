namespace Comanda.WebApi.Extensions;

public static class ServicesExtension
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.ConfigureSwagger();

        services.AddDataPersistence(configuration);
        services.ConfigureIdentity();

        services.AddMediator();
        services.AddValidation();
        services.AddMapping();

        services.AddJwtBearer(configuration);
        services.AddApplicationServices(configuration);
    }
}