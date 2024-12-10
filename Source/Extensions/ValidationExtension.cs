namespace Comanda.WebApi.Extensions;

[ExcludeFromCodeCoverage]
internal static class ValidationExtension
{
    public static void AddValidation(this IServiceCollection services)
    {
        #region validators for identity requests

        services.AddTransient<IValidator<AccountRegistrationRequest>, AccountRegistrationValidator>();
        services.AddTransient<IValidator<AuthenticationCredentials>, AuthenticationCredentialsValidator>();
        services.AddTransient<IValidator<SendPasswordResetTokenRequest>, SendPasswordResetTokenValidator>();
        services.AddTransient<IValidator<ResetPasswordRequest>, ResetPasswordValidator>();
        services.AddTransient<IValidator<AccountEditingRequest>, AccountEditingValidator>();

        #endregion


        #region validators for product requests

        services.AddTransient<IValidator<ProductCreationRequest>, ProductCreationValidator>();
        services.AddTransient<IValidator<ProductEditingRequest>, ProductEditingValidator>();

        #endregion

        #region validators for ingredient requests

        services.AddTransient<IValidator<IngredientCreationRequest>, IngredientCreationValidator>();
        services.AddTransient<IValidator<IngredientEditingRequest>, IngredientEditingValidator>();

        #endregion

        #region validators for additional requests

        services.AddTransient<IValidator<AdditionalCreationRequest>, AdditionalCreationValidator>();
        services.AddTransient<IValidator<AdditionalEditingRequest>, AdditionalEditingValidator>();

        #endregion

        #region validators for cart requests

        services.AddTransient<IValidator<UpdateItemQuantityInCartRequest>, UpdateItemQuantityValidator>();

        #endregion

        #region category-related validators

        services.AddTransient<IValidator<Category>, CategoryValidator>();
        services.AddTransient<IValidator<CategoryCreationRequest>, CategoryCreationValidator>();
        services.AddTransient<IValidator<CategoryEditingRequest>, CategoryEditingValidator>();

        #endregion

        #region validators for profile requests

        services.AddTransient<IValidator<NewAddressRegistrationRequest>, NewAddressRegistrationValidator>();
        services.AddTransient<IValidator<AddressEditingRequest>, AddressEditingValidator>();

        #endregion

        #region validators for settings requests

        services.AddTransient<IValidator<SettingsEditingRequest>, SettingsEditingValidator>();

        #endregion

        #region validators for coupon requests

        services.AddTransient<IValidator<CouponCreationRequest>, CouponCreationValidator>();
        services.AddTransient<IValidator<CouponEditingRequest>, CouponEditingValidator>();

        #endregion
    }
}