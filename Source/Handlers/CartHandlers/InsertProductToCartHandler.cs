namespace Comanda.WebApi.Handlers;

/*
    If Uncle Bob sees this code he'll surely have a heart attack. 
    If there's one thing this code doesn't respect, it's SRP, God forbid, what an ugly thing... 
    I'll refactor it later, making it work first is the most important thing. I'm agonized to see this.
*/

public sealed class InsertProductToCartHandler(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    IAdditionalRepository additionalRepository,
    IProductIngredientRepository productIngredientRepository,
    ICustomerRepository customerRepository,
    IUserContextService userContextService
) : IRequestHandler<InsertProductIntoCartRequest, Response>
{
    public async Task<Response> Handle(
        InsertProductIntoCartRequest request,
        CancellationToken cancellationToken
    )
    {
        var userIdentifier = userContextService.GetCurrentUserIdentifier();

        var product = await productRepository.RetrieveByIdAsync(request.ProductId);
        if (product is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Product not found."
            );

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

        var cart = await cartRepository.FindCartByCustomerIdAsync(customer.Id);

        if (cart is null)
        {
            cart = new Cart { Customer = customer };
            await cartRepository.SaveAsync(cart);
        }

        var cartItem = new CartItem(request.Quantity, product);
        foreach (var payload in request.Additionals)
        {
            var additional = await additionalRepository.RetrieveByIdAsync(payload.AdditionalId);
            if (additional is null)
                return new Response(
                    statusCode: StatusCodes.Status404NotFound,
                    message: "Additional not found."
                );

            cartItem.AddAdditional(additional, payload.Quantity);
        }

        foreach (var ingredientId in request.IngredientsIdsToRemove)
        {
            var ingredient = await productIngredientRepository.RetrieveByIdAsync(ingredientId);
            if (ingredient is null)
                return new Response(
                    statusCode: StatusCodes.Status404NotFound,
                    message: "Ingredient not found."
                );

            if (ingredient.IsMandatory)
                return new Response(
                    statusCode: StatusCodes.Status400BadRequest,
                    message: "Mandatory ingredients can't be removed."
                );
        }

        cart.AddItem(cartItem);
        await cartRepository.UpdateAsync(cart);

        return new Response(
            statusCode: StatusCodes.Status201Created,
            message: "Product added to cart successfully."
        );
    }
}