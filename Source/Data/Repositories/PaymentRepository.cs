namespace Comanda.WebApi.Data.Repositories;

public sealed class PaymentRepository(ComandaDbContext dbContext) :
    Repository<Payment, ComandaDbContext>(dbContext),
    IPaymentRepository
{
    #pragma warning disable CS8603
    public async Task<Payment> FindByOrderIdAsync(int orderId)
    {
        return await _dbContext.Payments
            .AsNoTracking()
            .Include(payment => payment.Order)
            .FirstOrDefaultAsync(payment => payment.Order.Id == orderId);
    }
}