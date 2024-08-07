namespace Comanda.WebApi.Handlers;

public sealed class CheckoutHandler(
    IUserContextService userContextService,
    ICartRepository cartRepository,
    ICustomerRepository customerRepository,
    IAddressRepository addressRepository,
    ICheckoutManager checkoutManager
) : IRequestHandler<CheckoutRequest, Response<CheckoutResponse>>
{
    public async Task<Response<CheckoutResponse>> Handle(
        CheckoutRequest request,
        CancellationToken token
    )
    {
        var userIdentifier = userContextService.GetCurrentUserIdentifier();

        /*
            It is impossible for the 'userIdentifier' to be null at this point
            since the 'CartController' restricts access only to authenticated customers.
        */
        #pragma warning disable CS8604, CS8602
 
        var customer = await customerRepository.FindCustomerByUserIdAsync(userIdentifier);
        var shippingAddress = await addressRepository.RetrieveByIdAsync(request.ShippingAddressId);
        var cart = await cartRepository.FindCartWithItemsAsync(customer.Id);

        if (shippingAddress is null)
            return new Response<CheckoutResponse>(
                data: null,
                statusCode: StatusCodes.Status404NotFound,
                message: "address with the specified id was not found."
            );

        var session = await checkoutManager.CreateCheckoutSessionAsync(cart, shippingAddress);
        var response = new CheckoutResponse { SessionId = session.Id, Url = session.Url };

        return new Response<CheckoutResponse>(
            data: response,
            statusCode: StatusCodes.Status200OK,
            message: "checkout section created successfully."
        );
    }
}