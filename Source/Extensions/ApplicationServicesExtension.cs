namespace Comanda.WebApi.Extensions;

#pragma warning disable CS8604
public static class ApplicationServicesExtension
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var smtpSettings = configuration.GetSection(nameof(SmtpSettings)).Get<SmtpSettings>();

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
        services.AddScoped<IConfirmationTokenService, ConfirmationTokenService>();
        services.AddScoped<IOrderHistoryFormatter, OrderHistoryFormatter>();
        services.AddScoped<IMenuFormatter, MenuFormatterService>();
        services.AddScoped<IGeminiService, GeminiService>();
        services.AddScoped<IRecommendationService, RecommendationService>();

        services.AddScoped<IEmailService, SmtpEmailService>(provider =>
        {
            return new SmtpEmailService(smtpSettings);
        });
    }
}