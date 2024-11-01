namespace Comanda.WebApi.Handlers;

public sealed class UpdateCartItemQuantityHandler(
    ICartRepository cartRepository,
    ICustomerRepository customerRepository,
    IUserContextService userContextService,
    IValidator<UpdateItemQuantityInCartRequest> validator
) : IRequestHandler<UpdateItemQuantityInCartRequest, Response>
{
    public async Task<Response> Handle(
        UpdateItemQuantityInCartRequest request,
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

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var itemToUpdate = cart.Items
            .FirstOrDefault(item => item.Id == request.ItemId);

        if (itemToUpdate is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Cart item not found."
            );

        itemToUpdate.Quantity = request.NewQuantity;

        await cartRepository.UpdateAsync(cart);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Cart item quantity updated successfully."
        );
    }
}
