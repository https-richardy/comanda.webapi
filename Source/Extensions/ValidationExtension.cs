namespace Comanda.WebApi.Extensions;

public static class ValidationExtension
{
    public static void AddValidation(this IServiceCollection services)
    {
        #region validators for accounts requests

        services.AddTransient<IValidator<AccountRegistrationRequest>, AccountRegistrationValidator>();

        #endregion

        #region  validators for establishments requests

        services.AddTransient<IValidator<CreateEstablishmentRequest>, CreateEstablishmentValidator>();
        services.AddTransient<IValidator<CreateEstablishmentProductRequest>, CreateEstablishmentProductValidator>();

        #endregion
    }
}