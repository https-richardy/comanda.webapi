namespace Comanda.WebApi.Data.Repositories;

public sealed class SettingsRepository(ComandaDbContext dbContext) :
    Repository<Settings, ComandaDbContext>(dbContext),
    ISettingsRepository
{
    #pragma warning disable CS8603
    public async Task<Settings> GetSettingsAsync()
    {
        var settings = await _dbContext.Settings.FirstOrDefaultAsync();
        return settings;
    }
}