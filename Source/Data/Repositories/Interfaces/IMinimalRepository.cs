namespace Comanda.WebApi.Data.Repositories;

/// <summary>
/// Minimal repository interface for basic CRUD operations on entities.
/// </summary>
/// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
public interface IMinimalRepository<TEntity>
{
    Task SaveAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);

    Task<TEntity> RetrieveByIdAsync(int id);
    Task<IEnumerable<TEntity>> RetrieveAllAsync();
}