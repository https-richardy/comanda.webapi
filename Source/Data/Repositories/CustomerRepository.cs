namespace Comanda.WebApi.Data.Repositories;

public sealed class CustomerRepository(ComandaDbContext dbContext) :
    Repository<Customer, ComandaDbContext>(dbContext),
    ICustomerRepository
{

}