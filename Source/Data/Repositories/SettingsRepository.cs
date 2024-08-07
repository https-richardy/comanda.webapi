namespace Comanda.WebApi.Data.Repositories;

public sealed class SettingsRepository(ComandaDbContext dbContext) :
    Repository<Settings, ComandaDbContext>(dbContext),
    ISettingsRepository
{

}