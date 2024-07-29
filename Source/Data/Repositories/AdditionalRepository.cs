namespace Comanda.WebApi.Data.Repositories;

public sealed class AdditionalRepository(ComandaDbContext dbContext) :
    MinimalRepository<Additional, ComandaDbContext>(dbContext),
    IAdditionalRepository
{

}