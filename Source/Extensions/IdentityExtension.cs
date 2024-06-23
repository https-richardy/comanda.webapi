namespace Comanda.WebApi.Extensions;

public static class IdentityExtension
{
    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<Account, IdentityRole>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ComandaDbContext>();
    }
}