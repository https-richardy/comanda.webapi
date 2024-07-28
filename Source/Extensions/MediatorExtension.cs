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

        #region handlers for cart requests

        services.AddScoped<IRequestHandler<InsertProductIntoCartRequest, Response>, InsertProductToCartHandler>();
        services.AddScoped<IRequestHandler<GetCartDetailsRequest, Response<CartResponse>>, CartDetailHandler>();
        services.AddScoped<IRequestHandler<UpdateItemQuantityInCartRequest, Response>, UpdateCartItemQuantityHandler>();

        #endregion

        #region handlers for product requests

        services.AddScoped<IRequestHandler<ProductDetailRequest, Response<Product>>, ProductDetailHandler>();
        services.AddScoped<IRequestHandler<ProductListingRequest, Response<PaginationHelper<Product>>>, ProductListingHandler>();
        services.AddScoped<IRequestHandler<ProductCreationRequest, Response>, ProductCreationHandler>();
        services.AddScoped<IRequestHandler<ProductEditingRequest, Response>, ProductEditingHandler>();
        services.AddScoped<IRequestHandler<ProductDeletionRequest, Response>, ProductDeletionHandler>();

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