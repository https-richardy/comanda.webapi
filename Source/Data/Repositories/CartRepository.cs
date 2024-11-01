namespace Comanda.WebApi.Data.Repositories;

public sealed class CartRepository(ComandaDbContext dbContext) :
    MinimalRepository<Cart, ComandaDbContext>(dbContext),
    ICartRepository
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
        /*
            To avoid the “DbUpdateConcurrencyException” concurrency exception that occurs when we
            we try to update or delete an entity that is no longer present in the database,
            it was necessary to ensure that all the entities involved are in the correct state for the
            the operation.
        */

        _dbContext.ChangeTracker.Clear();

        if (_dbContext.Entry(cart).State == EntityState.Detached)
            _dbContext.Carts.Attach(cart);

        if (_dbContext.Entry(item).State == EntityState.Detached)
            _dbContext.CartItems.Add(item);

        cart.Items.Add(item);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ClearCartAsync(Cart cart)
    {
        _dbContext.ChangeTracker.Clear();
        _dbContext.CartItems.RemoveRange(cart.Items);

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

    public async Task RemoveItemAsync(CartItem item)
    {
        _dbContext.CartItems.Remove(item);
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

    public async Task UpdateItemAsync(CartItem item)
    {
        _dbContext.CartItems.Update(item);
        await _dbContext.SaveChangesAsync();
    }
}