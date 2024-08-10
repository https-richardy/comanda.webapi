namespace Comanda.WebApi.Data.Repositories;

public interface IAddressRepository : IRepository<Address>
{
    Task<IEnumerable<Address>> GetAddressesByCustomerIdAsync(int customerId);
}