namespace Comanda.WebApi.Data.Repositories;

public interface ICartRepository : IMinimalRepository<Cart>
{
    Task AddItemAsync(Cart cart, CartItem item);
    Task RemoveItemAsync(Cart cart, CartItem item);
    Task ClearCartAsync(Cart cart);
    Task<Cart?> FindCartByCustomerIdAsync(int customerId);
    Task<Cart?> FindCartWithItemsAsync(int customerId);
}