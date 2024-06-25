namespace Comanda.WebApi.Extensions;

#pragma warning disable CS8604
public static class ApplicationServicesExtension
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAddressService, AddressService>();
        services.AddHttpClient<IAddressService, AddressService>(client =>
        {
            client.BaseAddress = new Uri(configuration["ExternalApis:ViaCepApiUrl"]);
        });

        services.AddFileUploadService();
    }
}