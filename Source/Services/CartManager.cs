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

        var cart = await cartRepository.FindCartByCustomerIdAsync(customerId) ?? new Cart { Customer = customer };

        var establishment = await establishmentRepository.RetrieveByIdAsync(establishmentId);
        if (establishment is null)
            throw new EstablishmentNotFoundException(establishmentId);

        var product = await establishmentRepository.GetProductByIdAsync(productId, establishmentId);
        if (product is null)
            throw new EstablishmentProductNotFoundException(establishmentId, productId);

        cart.AddItem(product, quantity);
        await cartRepository.AddItemAsync(cart, BuildCartItem(product, quantity));
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