namespace Comanda.WebApi.Data.Repositories;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment> FindByOrderIdAsync(int orderId);
}