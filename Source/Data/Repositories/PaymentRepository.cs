namespace Comanda.WebApi.Data.Repositories;

public sealed class PaymentRepository(ComandaDbContext dbContext) :
    Repository<Payment, ComandaDbContext>(dbContext),
    IPaymentRepository
{

}