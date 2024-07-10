namespace Comanda.WebApi.Extensions;

public static class ValidationExtension
{
    public static void AddValidation(this IServiceCollection services)
    {
        #region validators for identity requests

        services.AddTransient<IValidator<AccountRegistrationRequest>, AccountRegistrationValidator>();
        services.AddTransient<IValidator<AuthenticationCredentials>, AuthenticationCredentialsValidator>();

        #endregion
    }
}