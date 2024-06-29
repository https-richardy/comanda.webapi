namespace Comanda.WebApi.Data.Repositories;

public sealed class CartRepository(ComandaDbContext dbContext) :
    MinimalRepository<Cart, ComandaDbContext>(dbContext), ICartRepository
{
    public async Task AddItemAsync(Cart cart, CartItem item)
    {
        cart.Items.Add(item);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ClearCartAsync(Cart cart)
    {
        cart.Items.Clear();
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Cart?> FindCartByCustomerIdAsync(int customerId)
    {
        var cart = await _dbContext.Carts
            .Where(cart => cart.Customer.Id == customerId)
            .FirstOrDefaultAsync();

        return cart;
    }

    public async Task RemoveItemAsync(Cart cart, CartItem item)
    {
        cart.Items.Remove(item);
        await _dbContext.SaveChangesAsync();
    }
}