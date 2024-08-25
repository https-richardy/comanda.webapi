namespace Comanda.WebApi.Data.Repositories;

public sealed class CartRepository(ComandaDbContext dbContext) :
    MinimalRepository<Cart, ComandaDbContext>(dbContext), ICartRepository
{
    public override async Task SaveAsync(Cart entity)
    {
        // Ensure the entity state is set to Added to avoid the IDENTITY_INSERT error
        // when saving a new Cart entity without explicitly setting the ID.

        _dbContext.Entry(entity).State = EntityState.Added;
        await _dbContext.SaveChangesAsync();
    }

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

    public async Task<Cart?> FindCartWithItemsAsync(int customerId)
    {
        /*
            If someone ever asks you for an example of overfetching, show them this query.
            I swear I'll optimize this query later...
        */
        return await _dbContext.Carts
            .AsNoTracking()
            .Include(cart => cart.Items)
            .ThenInclude(cartItem => cartItem.Product)
            .Include(cart => cart.Items)
            .ThenInclude(cartItem => cartItem.Additionals)
            .ThenInclude(additional => additional.Additional)
            .FirstOrDefaultAsync(cart => cart.Customer.Id == customerId);
    }

    public async Task<Cart?> FindCartByCustomerIdAsync(int customerId)
    {
        var cart = await _dbContext.Carts
            .AsNoTracking()
            .Where(cart => cart.Customer.Id == customerId)
            .FirstOrDefaultAsync();

        return cart;
    }

    public async Task RemoveItemAsync(Cart cart, CartItem item)
    {
        cart.Items.Remove(item);
        await _dbContext.SaveChangesAsync();
    }

    #pragma warning disable CS8603
    public override async Task<Cart> RetrieveByIdAsync(int id)
    {
        return await _dbContext.Carts
            .AsNoTracking()
            .Include(cart => cart.Customer)
            .Include(cart => cart.Items)
            .ThenInclude(cartItem => cartItem.Product)
            .Include(cart => cart.Items)
            .ThenInclude(cartItem => cartItem.Additionals)
            .ThenInclude(additional => additional.Additional)
            .FirstOrDefaultAsync(cart => cart.Id == id);
    }
}