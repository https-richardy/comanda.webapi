namespace Comanda.WebApi.Data.Repositories;

public sealed class PaymentRepository(ComandaDbContext dbContext) :
    Repository<Payment, ComandaDbContext>(dbContext),
    IPaymentRepository
{
    public override async Task SaveAsync(Payment entity)
    {
        // Ensure the entity state is set to Added to avoid the IDENTITY_INSERT error
        // when saving a new Payment entity without explicitly setting the ID.

        _dbContext.Entry(entity).State = EntityState.Added;
        await _dbContext.SaveChangesAsync();
    }

#pragma warning disable CS8603
    public async Task<Payment> FindByOrderIdAsync(int orderId)
    {
        return await _dbContext.Payments
            .AsNoTracking()
            .Include(payment => payment.Order)
            .FirstOrDefaultAsync(payment => payment.Order.Id == orderId);
    }
}