namespace Comanda.WebApi.Data.Repositories;

/// <summary>
/// Repository class providing extended query operations for entities using Entity Framework.
/// </summary>
/// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
/// <typeparam name="TDbContext">The type of the Entity Framework DbContext.</typeparam>
/// <remarks>
/// <para>
/// The <see cref="Repository{TEntity, TDbContext}"/> class is an extension of the <see cref="MinimalRepository{TEntity, TDbContext}"/>,
/// building upon its foundation to offer additional query operations for Entity Framework entities.
/// </para>
/// <para>
/// While <see cref="MinimalRepository{TEntity, TDbContext}"/> provides basic CRUD operations, 
/// <see cref="Repository{TEntity, TDbContext}"/> enhances functionality by incorporating advanced querying capabilities.
/// </para>
/// <para>
/// It is suitable for scenarios where a more comprehensive set of query operations is needed beyond simple Create, Read, Update, and Delete (CRUD).
/// </para>
/// </remarks>
public abstract class Repository<TEntity, TDbContext> : MinimalRepository<TEntity, TDbContext>, IRepository<TEntity>
    where TEntity : Entity
    where TDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{TEntity, TDbContext}"/> class.
    /// </summary>
    /// <param name="dbContext">The Entity Framework DbContext instance.</param>
    protected Repository(TDbContext dbContext)
        : base(dbContext) {  }

    /// <summary>
    /// Counts the total number of entities in the repository.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, returning the total count of entities.</returns>
    public virtual async Task<int> CountAsync()
    {
        var result = await _dbContext.Set<TEntity>().CountAsync();
        return result;
    }

    /// <summary>
    /// Counts the number of entities in the repository based on the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <returns>A task representing the asynchronous operation, returning the count of entities matching the predicate.</returns>
    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var result = await _dbContext.Set<TEntity>().CountAsync(predicate);
        return result;
    }

    /// <summary>
    /// Checks if an entity with the specified ID exists in the repository.
    /// </summary>
    /// <param name="id">The entity's indentifier.</param>
    /// <returns>A task representing the asynchronous operation, returning true if the entity exists, false otherwise.</returns>
    public virtual async Task<bool> ExistsAsync(object id)
    {
        var existingEntity = await _dbContext.Set<TEntity>().FindAsync(id);
        return existingEntity != null;
    }

    /// <summary>
    /// Finds all entities based on the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of entities.</returns>
    public virtual async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbContext.Set<TEntity>()
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();
    }

    /// <summary>
    /// Finds a single entity based on the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <returns>A task representing the asynchronous operation, returning the found entity or null if not found.</returns>
    # pragma warning disable CS8603
    public virtual async Task<TEntity> FindSingleAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
    }
    # pragma warning restore CS8603

    /// <summary>
    /// Retrieves a paged collection of entities.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of entities per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a paged collection of entities.</returns>
    public virtual async Task<IEnumerable<TEntity>> PagedAsync(int pageNumber, int pageSize)
    {
        /* Represents the adjustment applied to page numbers to align with zero-based indices in LINQ queries. */
        const int pageIndexAdjustment = 1;

        return await _dbContext.Set<TEntity>()
            .Skip((pageNumber - pageIndexAdjustment) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a paged collection of entities based on the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of entities per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a paged collection of entities.</returns>
    public virtual async Task<IEnumerable<TEntity>> PagedAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize)
    {
        /* Represents the adjustment applied to page numbers to align with zero-based indices in LINQ queries. */
        const int pageIndexAdjustment = 1;

        return await _dbContext.Set<TEntity>()
            .Where(predicate)
            .Skip((pageNumber - pageIndexAdjustment) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}