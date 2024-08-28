namespace Comanda.WebApi.Extensions;

[ExcludeFromCodeCoverage]
internal static class IdentityExtension
{
    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<Account, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ ";
        })
        .AddDefaultTokenProviders()
        .AddEntityFrameworkStores<ComandaDbContext>();
    }
}