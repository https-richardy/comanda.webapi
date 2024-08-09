namespace Comanda.WebApi.Extensions;

#pragma warning disable CS8604
public static class ApplicationServicesExtension
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAddressService, AddressService>();
        services.AddHttpClient<IAddressService, AddressService>(client =>
        {
            client.BaseAddress = new Uri(configuration["ExternalApis:ViaCep"]);
        });

        services.AddFileUploadService();

        services.AddScoped<IHostInformationProvider, HostInformationProvider>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddScoped<ICheckoutManager, CheckoutManager>();
        services.AddScoped<IRefundManager, RefundManager>();
    }
}