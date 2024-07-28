namespace Comanda.WebApi.Handlers;

public sealed class ModifyItemQuantityHandler(
    ICartRepository cartRepository,
    ICustomerRepository customerRepository,
    IUserContextService userContextService,
    IValidator<ModifyCartItemQuantityRequest> validator
) : IRequestHandler<ModifyCartItemQuantityRequest, Response>
{
    public async Task<Response> Handle(
        ModifyCartItemQuantityRequest request,
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
            .FirstOrDefault(item => item.Product.Id == request.ProductId);

        if (itemToUpdate is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Cart item not found."
            );

        if (request.Action == ChangeItemQuantityAction.Increment)
            itemToUpdate.Quantity++;
        /*  TODO:
            Here a logical error occurs, allowing a decrement to 0 or negative numbers.
            Should correct it to allow decrementing to 1 only.
        */
        else
            itemToUpdate.Quantity--;

        await cartRepository.UpdateAsync(cart);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Cart item quantity updated successfully."
        );
    }
}