namespace Comanda.WebApi.Data.Repositories;

public sealed class AddressRepository(ComandaDbContext dbContext) :
    Repository<Address, ComandaDbContext>(dbContext),
    IAddressRepository
{
    public async Task<IEnumerable<Address>> GetAddressesByCustomerIdAsync(int customerId)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .Where(customer => customer.Id == customerId)
            .SelectMany(customer => customer.Addresses.Where(address => address.IsDeleted == false))
            .ToListAsync();
    }
}