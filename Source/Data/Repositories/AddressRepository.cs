namespace Comanda.WebApi.Data.Repositories;

public sealed class AddressRepository(ComandaDbContext dbContext) :
    Repository<Address, ComandaDbContext>(dbContext),
    IAddressRepository
{
    
}