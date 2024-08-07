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
        services.AddScoped<IRequestHandler<CartItemDeletionRequest, Response>, CartItemDeletionHandler>();
        services.AddScoped<IRequestHandler<IncrementCartItemQuantityRequest, Response>, IncrementCartItemQuantityHandler>();
        services.AddScoped<IRequestHandler<DecrementCartItemQuantityRequest, Response>, DecrementCartItemQuantityHandler>();
        services.AddScoped<IRequestHandler<RetrieveCartAndAddressRequest, (Cart cart, Address addres)>, RetrieveCartAndAddressHandler>();

        #endregion

        #region handlers for checkout requests

        services.AddScoped<IRequestHandler<CheckoutRequest, Response<CheckoutResponse>>, CheckoutHandler>();
        services.AddScoped<IRequestHandler<SuccessfulPaymentRequest, Response>, SuccessfulPaymentHandler>();
        services.AddScoped<IRequestHandler<FetchPaymentSessionRequest, Session>, FetchPaymentSessionHandler>();

        #endregion

        #region handlers for orders requests

        services.AddScoped<IRequestHandler<OrderProcessingRequest, Order>, OrderProcessingHandler>();

        #endregion

        #region handlers for product requests

        services.AddScoped<IRequestHandler<ProductDetailRequest, Response<Product>>, ProductDetailHandler>();
        services.AddScoped<IRequestHandler<ProductListingRequest, Response<PaginationHelper<Product>>>, ProductListingHandler>();
        services.AddScoped<IRequestHandler<ProductCreationRequest, Response<ProductCreationResponse>>, ProductCreationHandler>();
        services.AddScoped<IRequestHandler<ProductEditingRequest, Response>, ProductEditingHandler>();
        services.AddScoped<IRequestHandler<ProductImageUploadRequest, Response>, ProductImageUploadHandler>();
        services.AddScoped<IRequestHandler<ProductDeletionRequest, Response>, ProductDeletionHandler>();

        #endregion

        #region handlers for ingredient requests

        services.AddScoped<IRequestHandler<IngredientListingRequest, Response<IEnumerable<Ingredient>>>, IngredientListingHandler>();
        services.AddScoped<IRequestHandler<IngredientCreationRequest, Response>, IngredientCreationHandler>();
        services.AddScoped<IRequestHandler<IngredientEditingRequest, Response>, IngredientEditingHandler>();
        services.AddScoped<IRequestHandler<IngredientDeletionRequest, Response>, IngredientDeletionHandler>();

        #endregion

        #region handlers for additional requests

        services.AddScoped<IRequestHandler<AdditionalsListingRequest, Response<IEnumerable<Additional>>>, AdditionalsListingHandler>();
        services.AddScoped<IRequestHandler<AdditionalsListingByCategoryRequest, Response<IEnumerable<Additional>>>, AdditionalsListingByCategoryHandler>();
        services.AddScoped<IRequestHandler<AdditionalCreationRequest, Response>, AdditionalCreationHandler>();
        services.AddScoped<IRequestHandler<AdditionalEditingRequest, Response>, AdditionalEditingHandler>();
        services.AddScoped<IRequestHandler<AdditionalDeletionRequest, Response>, AdditionalDeletionHandler>();

        #endregion

        #region handlers for category requests

        services.AddScoped<IRequestHandler<CategoryDetailRequest, Response<Category>>, CategoryDetailHandler>();
        services.AddScoped<IRequestHandler<CategoryListingRequest, Response<IEnumerable<Category>>>, CategoryListingHandler>();
        services.AddScoped<IRequestHandler<CategoryCreationRequest, Response>, CategoryCreationHandler>();
        services.AddScoped<IRequestHandler<CategoryEditingRequest, Response>, CategoryEditingHandler>();
        services.AddScoped<IRequestHandler<CategoryDeletionRequest, Response>, CategoryDeletionHandler>();

        #endregion

        #region handlers for profile requests

        services.AddScoped<IRequestHandler<NewAddressRegistrationRequest, Response>, NewAddressRegistrationHandler>();

        #endregion

        #region handlers for settings requests

        services.AddScoped<IRequestHandler<SettingsDetailsRequest, Response<SettingsFormattedResponse>>, SettingsDetailsHandler>();
        services.AddScoped<IRequestHandler<SettingsEditingRequest, Response>, SettingsEditingHandler>();

        #endregion
    }
}