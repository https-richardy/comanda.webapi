namespace Comanda.WebApi.Handlers;

public sealed class IncrementCartItemQuantityHandler(
    ICartRepository cartRepository,
    ICustomerRepository customerRepository,
    IUserContextService userContextService
) : IRequestHandler<IncrementCartItemQuantityRequest, Response>
{
    public async Task<Response> Handle(
        IncrementCartItemQuantityRequest request,
        CancellationToken cancellationToken
    )
    {
        var userIdentifier = userContextService.GetCurrentUserIdentifier();

        /*
            It is impossible for the 'userIdentifier' to be null at this point
            since the 'CartController' restricts access only to authenticated customers.
        */
        #pragma warning disable CS8604, CS8602

        var customer = await customerRepository.FindCustomerByUserIdAsync(userIdentifier);

        /*
            Even here it can't be null because when we create an account
            we have already created and associated a customer with it.
        */

        var cart = await cartRepository.FindCartWithItemsAsync(customer.Id);

        var itemToIncrement = cart.Items
            .FirstOrDefault(item => item.Id == request.ItemId);

        if (itemToIncrement is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Cart item not found."
            );

        itemToIncrement.Quantity++;

        await cartRepository.UpdateItemAsync(itemToIncrement);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Cart item quantity updated successfully."
        );
    }
}