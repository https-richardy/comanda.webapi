namespace Comanda.WebApi.Extensions;

public static class ValidationExtension
{
    public static void AddValidation(this IServiceCollection services)
    {
        #region validators for identity requests

        services.AddTransient<IValidator<AccountRegistrationRequest>, AccountRegistrationValidator>();
        services.AddTransient<IValidator<AuthenticationCredentials>, AuthenticationCredentialsValidator>();

        #endregion


        #region validators for product requests

        services.AddTransient<IValidator<ProductCreationRequest>, ProductCreationValidator>();
        services.AddTransient<IValidator<ProductEditingRequest>, ProductEditingValidator>();

        #endregion

        #region validators for category requests

        services.AddTransient<IValidator<CategoryCreationRequest>, CategoryCreationValidator>();
        services.AddTransient<IValidator<CategoryEditingRequest>, CategoryEditingValidator>();

        #endregion
    }
}