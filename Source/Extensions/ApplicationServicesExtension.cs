namespace Comanda.WebApi.Extensions;

[ExcludeFromCodeCoverage]
internal static class ApplicationServicesExtension
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var smtpSettings = configuration.GetSection(nameof(SmtpSettings)).Get<SmtpSettings>();

        services.AddScoped<IAddressManager, AddressManager>();
        services.AddHttpClient<IAddressManager, AddressManager>(client =>
        {
            client.BaseAddress = new Uri(configuration["ExternalApis:ViaCep"]!);
        });

        services.AddFileUploadService();

        services.AddScoped<ICategoryManager, CategoryManager>();
        services.AddScoped<IHostInformationProvider, HostInformationProvider>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddScoped<ICheckoutManager, CheckoutManager>();
        services.AddScoped<IRefundManager, RefundManager>();
        services.AddScoped<IConfirmationTokenService, ConfirmationTokenService>();
        services.AddScoped<ICouponService, CouponService>();
        services.AddScoped<IOrderHistoryFormatter, OrderHistoryFormatter>();
        services.AddScoped<IMenuFormatter, MenuFormatterService>();
        services.AddScoped<IGeminiService, GeminiService>();
        services.AddScoped<IRecommendationService, RecommendationService>();
        services.AddScoped<IProfileDataExportService, ProfileDataExportService>();

        services.AddScoped<IEmailService, SmtpEmailService>(provider =>
        {
            return new SmtpEmailService(smtpSettings!);
        });

        services.AddScoped<SeedService>();
    }
}