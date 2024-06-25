namespace Comanda.WebApi.Data.Repositories;

public sealed class EstablishmentRepository(ComandaDbContext dbContext) : 
    Repository<Establishment, ComandaDbContext>(dbContext),
    IEstablishmentRepository
{
    
}