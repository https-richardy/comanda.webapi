namespace Comanda.WebApi.Services;

public sealed class CartManager(
    ICartRepository cartRepository,
    ICustomerRepository customerRepository,
    IEstablishmentRepository establishmentRepository
)
{
    public async Task AddItemAsync(int customerId, int productId, int quantity, int establishmentId)
    {
        var customer = await customerRepository.RetrieveByIdAsync(customerId);
        if (customer is null)
            throw new CustomerNotFoundException(customerId);

        var cart = await cartRepository.FindCartByCustomerIdAsync(customerId);
        if (cart is null)
        {
            cart = new Cart { Customer = customer };
            await cartRepository.SaveAsync(cart);
        }

        var establishment = await establishmentRepository.RetrieveByIdAsync(establishmentId);
        if (establishment is null)
            throw new EstablishmentNotFoundException(establishmentId);

        var product = await establishmentRepository.GetProductByIdAsync(productId, establishmentId);
        if (product is null)
            throw new EstablishmentProductNotFoundException(establishmentId, productId);

        await cartRepository.AddItemAsync(cart, BuildCartItem(product, quantity));
    }

    public async Task<CartResponse> GetCustomerCartDetailsAsync(int customerId)
    {
        var customer = await customerRepository.RetrieveByIdAsync(customerId);
        if (customer is null)
            throw new CustomerNotFoundException(customerId);

        var cart = await cartRepository.FindCartWithItemsAsync(customerId);
        if (cart is null)
            return new CartResponse();

        var formattedItems = cart.Items.Select(cartItem => new FormattedCartItem
        {
            Id = cartItem.Product.Id,
            Title = cartItem.Product.Title,
            ImageUrl = cartItem.Product.ImagePath,
            Price = cartItem.Product.Price,
            Quantity = cartItem.Quantity
        }).ToList();

        var cartResponse = new CartResponse
        {
            Total = cart.Total,
            Items = formattedItems
        };

        return cartResponse;
    }

    public async Task<Cart> GetCartAsync(int customerId)
    {
        var cart = await cartRepository.FindCartByCustomerIdAsync(customerId);
        if (cart is null)
            throw new CartNotFoundException(customerId);

        return cart;
    }

    public async Task<decimal> GetCartTotalAsync(int customerId)
    {
        var cart = await cartRepository.FindCartByCustomerIdAsync(customerId);
        if (cart is null)
            throw new CartNotFoundException(customerId);

        return cart.Total;
    }

    public async Task ClearCartAsync(int customerId)
    {
        var cart = await cartRepository.FindCartByCustomerIdAsync(customerId);
        if (cart is null)
            throw new CartNotFoundException(customerId);

        await cartRepository.ClearCartAsync(cart);
    }

    private CartItem BuildCartItem(Product product, int quantity)
    {
        return new CartItem
        {
            Product = product,
            Quantity = quantity
        };
    }
}