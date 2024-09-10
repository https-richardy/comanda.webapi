namespace Comanda.WebApi.Extensions;

[ExcludeFromCodeCoverage]
internal static class MediatorExtension
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
        services.AddScoped<IRequestHandler<SendPasswordResetTokenRequest, Response>, SendPasswordResetTokenHandler>();
        services.AddScoped<IRequestHandler<ResetPasswordRequest, Response>, ResetPasswordHandler>();

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

        #region handlers for payment requests

        services.AddScoped<IRequestHandler<PaymentProcessingRequest, Payment>, PaymentProcessingHandler>();

        #endregion

        #region handlers for checkout requests

        services.AddScoped<IRequestHandler<CheckoutRequest, Response<CheckoutResponse>>, CheckoutHandler>();
        services.AddScoped<IRequestHandler<SuccessfulPaymentRequest, Response>, SuccessfulPaymentHandler>();
        services.AddScoped<IRequestHandler<FetchPaymentSessionRequest, Session>, FetchPaymentSessionHandler>();
        services.AddScoped<IRequestHandler<CanceledPaymentRequest, Response>, CanceledPaymentHandler>();

        #endregion

        #region handlers for orders requests

        services.AddScoped<IRequestHandler<OrderProcessingRequest, Order>, OrderProcessingHandler>();
        services.AddScoped<IRequestHandler<SetOrderStatusRequest, Response>, SetOrderStatusHandler>();
        services.AddScoped<IRequestHandler<FetchOrdersRequest, Response<PaginationHelper<FormattedOrder>>>, FetchOrdersHandler>();
        services.AddScoped<IRequestHandler<OrderDetailsRequest, Response<FormattedOrderDetails>>, FetchOrderHandler>();
        services.AddScoped<IRequestHandler<OrderCancellationRequest, Response>, OrderCancellationHandler>();

        #endregion

        #region handlers for product requests

        services.AddScoped<IRequestHandler<ProductDetailRequest, Response<Product>>, ProductDetailHandler>();
        services.AddScoped<IRequestHandler<ProductListingRequest, Response<PaginationHelper<FormattedProduct>>>, ProductListingHandler>();
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

        services.AddScoped<IRequestHandler<FetchCustomerAddressesRequest, Response<IEnumerable<Address>>>, FetchCustomerAddressesHandler>();
        services.AddScoped<IRequestHandler<NewAddressRegistrationRequest, Response>, NewAddressRegistrationHandler>();
        services.AddScoped<IRequestHandler<AddressEditingRequest, Response>, AddressEditingHandler>();
        services.AddScoped<IRequestHandler<AddressDeletionRequest, Response>, AddressDeletionHandler>();
        services.AddScoped<IRequestHandler<CustomerOrderHistoryRequest, Response<PaginationHelper<FormattedOrder>>>, CustomerOrdersHistoryHandler>();
        services.AddScoped<IRequestHandler<CustomerCurrentOrdersRequest, Response<IEnumerable<FormattedOrder>>>, CustomerCurrentOrdersHandler>();
        services.AddScoped<IRequestHandler<CustomerOrderDetailsRequest, Response<FormattedOrderDetails>>, CustomerOrderDetailsHandler>();

        #endregion

        #region handlers for settings requests

        services.AddScoped<IRequestHandler<SettingsDetailsRequest, Response<SettingsFormattedResponse>>, SettingsDetailsHandler>();
        services.AddScoped<IRequestHandler<SettingsEditingRequest, Response>, SettingsEditingHandler>();

        #endregion

        #region handlers for recommendation requests

        services.AddScoped<IRequestHandler<RecommendationRequest, Response<RecommendationResponse>>, RecommendationHandler>();

        #endregion

        #region handlers for coupons requests

        services.AddScoped<IRequestHandler<CouponCreationRequest, Response>, CouponCreationHandler>();
        services.AddScoped<IRequestHandler<CouponListingRequest, Response<IEnumerable<Coupon>>>, CouponListingHandler>();
        services.AddScoped<IRequestHandler<FetchCouponByIdentifier, Response<Coupon>>, FetchCouponByIdentifierHandler>();
        services.AddScoped<IRequestHandler<FetchCouponByCodeRequest, Response<Coupon>>, FetchCouponByCodeHandler>();
        services.AddScoped<IRequestHandler<CouponEditingRequest, Response>, CouponEditingHandler>();
        services.AddScoped<IRequestHandler<CouponDeletionRequest, Response>, CouponDeletionHandler>();

        #endregion
    }
}