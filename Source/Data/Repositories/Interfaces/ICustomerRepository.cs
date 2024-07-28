namespace Comanda.WebApi.Data.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> FindCustomerByUserIdAsync(string userId);
}