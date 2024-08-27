namespace Comanda.WebApi.Data.Repositories;

public sealed class CustomerRepository(ComandaDbContext dbContext) :
    Repository<Customer, ComandaDbContext>(dbContext),
    ICustomerRepository
{
    public async Task<Customer?> FindCustomerByUserIdAsync(string userId)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .Include(customer => customer.Account)
            .Include(customer => customer.Orders)
            .Include(customer => customer.Addresses)
            .Where(customer => customer.Account.Id == userId)
            .FirstOrDefaultAsync();
    }
}