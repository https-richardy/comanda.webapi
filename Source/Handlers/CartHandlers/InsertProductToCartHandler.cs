namespace Comanda.WebApi.Handlers;

public sealed class InsertProductToCartHandler(
    ICartRepository cartRepository,
    IProductRepository productRepository,
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

        cart.AddItem(product, request.Quantity);
        await cartRepository.UpdateAsync(cart);

        return new Response(
            statusCode: StatusCodes.Status201Created,
            message: "Product added to cart successfully."
        );
    }
}