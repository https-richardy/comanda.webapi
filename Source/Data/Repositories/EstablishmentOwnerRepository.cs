namespace Comanda.WebApi.Data.Repositories;

public sealed class EstablishmentOwnerRepository(ComandaDbContext dbContext) :
    Repository<EstablishmentOwner, ComandaDbContext>(dbContext),
    IRepository<EstablishmentOwner>
{

}