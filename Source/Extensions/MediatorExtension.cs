namespace Comanda.WebApi.Extensions;

public static class MediatorExtension
{
    public static void AddMediator(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        #region handlers for identity requests

        services.AddScoped<IRequestHandler<AccountRegistrationRequest, Response>, AccountRegistrationHandler>();
        services.AddScoped<IRequestHandler<AuthenticationCredentials, Response<AuthenticationResponse>>, AuthenticationHandler>();

        #endregion

        #region handlers for category requests

        services.AddScoped<IRequestHandler<CategoryDetailRequest, Response<Category>>, CategoryDetailHandler>();
        services.AddScoped<IRequestHandler<CategoryListingRequest, Response<IEnumerable<Category>>>, CategoryListingHandler>();
        services.AddScoped<IRequestHandler<CategoryCreationRequest, Response>, CategoryCreationHandler>();
        services.AddScoped<IRequestHandler<CategoryEditingRequest, Response>, CategoryEditingHandler>();
        services.AddScoped<IRequestHandler<CategoryDeletionRequest, Response>, CategoryDeletionHandler>();

        #endregion
    }
}