namespace Comanda.WebApi.Data.Repositories;

/// <summary>
/// Extension of the <see cref="IMinimalRepository{TEntity}"/> interface providing additional query operations.
/// </summary>
/// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
public interface IRepository<TEntity> : IMinimalRepository<TEntity>
{
    /// <summary>
    /// Finds a single entity based on the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning the found entity or null if not found.
    /// </returns>
    Task<TEntity> FindSingleAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Finds all entities based on the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning a collection of entities.
    /// </returns>
    Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Retrieves a paged collection of entities.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of entities per page.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning a paged collection of entities.
    /// </returns>
    Task<IEnumerable<TEntity>> PagedAsync(int pageNumber, int pageSize);

    /// <summary>
    /// Retrieves a paged collection of entities based on the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of entities per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a paged collection of entities.</returns>
    Task<IEnumerable<TEntity>> PagedAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize);

    /// <summary>
    /// Checks if an entity with the specified ID exists in the repository.
    /// </summary>
    /// <param name="id">The entity's indentifier.</param>
    /// <returns>A task representing the asynchronous operation, returning true if the entity exists, false otherwise.</returns>
    Task<bool> ExistsAsync(object id);

    /// <summary>
    /// Counts the total number of entities in the repository.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, returning the total count of entities.</returns>
    Task<int> CountAsync();

    /// <summary>
    /// Counts the number of entities in the repository based on the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <returns>A task representing the asynchronous operation, returning the count of entities matching the predicate.</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
}