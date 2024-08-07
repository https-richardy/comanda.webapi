namespace Comanda.WebApi.Data.Repositories;

public interface ISettingsRepository : IRepository<Settings>
{
    Task<Settings> GetSettingsAsync();
}