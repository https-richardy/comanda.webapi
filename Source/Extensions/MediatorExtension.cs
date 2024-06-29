namespace Comanda.WebApi.Extensions;

public static class MediatorExtension
{
    public static void AddMediator(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        #region handlers for accounts requests

        services.AddScoped<IRequestHandler<AccountRegistrationRequest, Response>, AccountRegistrationHandler>();
        services.AddScoped<IRequestHandler<AuthenticationRequest, Response<AuthenticationResponse>>, AuthenticationHandler>();

        #endregion

        #region handlers for establishments requests

        services.AddScoped<IRequestHandler<CreateEstablishmentRequest, Response>, CreateEstablishmentHandler>();
        services.AddScoped<IRequestHandler<CreateEstablishmentProductRequest, Response>, CreateEstablishmentProductHandler>();
        services.AddScoped<IRequestHandler<EstablishmentCategoryRegistrationRequest, Response>, EstablishmentCategoryRegistrationHandler>();
        services.AddScoped<IRequestHandler<GetEstablishmentProductsRequest, Response<PaginationHelper<Product>>>, GetEstablishmentProductsHandler>();

        #endregion

        #region handlers for cart requests

        services.AddScoped<IRequestHandler<AddProductToCartRequest, Response>, AddProductToCartHandler>();

        #endregion
    }
}