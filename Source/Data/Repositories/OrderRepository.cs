namespace Comanda.WebApi.Data.Repositories;

public sealed class OrderRepository(ComandaDbContext dbContext) :
    Repository<Order, ComandaDbContext>(dbContext),
    IOrderRepository
{

}
