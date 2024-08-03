namespace Comanda.WebApi.Handlers;

public sealed class CartItemDeletionHandler(
    ICartRepository cartRepository,
    ICustomerRepository customerRepository,
    IUserContextService userContextService
) : IRequestHandler<CartItemDeletionRequest, Response>
{
    public async Task<Response> Handle(
        CartItemDeletionRequest request,
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

        if (cart is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Cart not found."
            );

        var itemToRemove = cart.Items.FirstOrDefault(item => item.Product.Id == request.ItemId);

        if (itemToRemove is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Item not found in cart."
            );

        cart.Items.Remove(itemToRemove);
        await cartRepository.UpdateAsync(cart);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Item removed from cart successfully."
        );
    }
}
