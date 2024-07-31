namespace Comanda.WebApi.Handlers;

public sealed class CartDetailHandler(
    ICartRepository cartRepository,
    ICustomerRepository customerRepository,
    IUserContextService userContextService
) : IRequestHandler<GetCartDetailsRequest, Response<CartResponse>>
{
    public async Task<Response<CartResponse>> Handle(
        GetCartDetailsRequest request,
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
        var cart = await cartRepository.FindCartWithItemsAsync(customer.Id);

        if (cart is null)
        {
            cart = new Cart { Customer = customer };
            await cartRepository.SaveAsync(cart);
        }

        if (cart.Items.Count == 0)
        {
            return new Response<CartResponse>(
                data: null,
                statusCode: StatusCodes.Status200OK,
                message: "Cart is empty."
            );
        }

        var formattedItems = new List<FormattedCartItem>();
        foreach (var item in cart.Items)
        {
            formattedItems.Add(new FormattedCartItem
            {
                Id = item.Product.Id,
                Title = item.Product.Title,
                Price = item.Product.Price,
                ImageUrl = item.Product?.ImagePath ?? string.Empty,
                Quantity = item.Quantity
            });
        }

        var response = new CartResponse
        {
            Total = cart.Total,
            Items = formattedItems
        };

        return new Response<CartResponse>(
            data: response,
            statusCode: StatusCodes.Status200OK,
            message: "Cart retrieved successfully."
        );
    }
}