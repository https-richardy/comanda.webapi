namespace Comanda.WebApi.Data.Repositories;

/// <summary>
/// A generic repository providing basic CRUD operations for entities using Entity Framework.
/// </summary>
/// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
/// <typeparam name="TDbContext">The type of the Entity Framework DbContext.</typeparam>
/// <remarks>
/// The <see cref="MinimalRepository{TEntity, TDbContext}"/> serves as a generic repository implementation
/// for handling basic CRUD (Create, Read, Update, Delete) operations for entities in the application.
/// This repository is designed to work with Entity Framework, and the type parameters allow flexibility in
/// choosing the entity type, the type of its primary key, and the DbContext type for data access.
/// </remarks>
public abstract class MinimalRepository<TEntity, TDbContext> : IMinimalRepository<TEntity>
    where TEntity : Entity
    where TDbContext : DbContext
{
    /// <summary>
    /// The Entity Framework DbContext for database interactions.
    /// </summary>
    protected readonly TDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="MinimalRepository{TEntity, TDbContext}"/> class.
    /// </summary>
    /// <param name="dbContext">The Entity Framework DbContext instance.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public MinimalRepository(TDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public virtual async Task SaveAsync(TEntity entity)
    {
        await _dbContext.Set<TEntity>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        var existingEntity = await _dbContext.Set<TEntity>().FindAsync(entity.Id);

        if (existingEntity != null)
        {
            _dbContext.Entry(existingEntity).State = EntityState.Detached;
            _dbContext.Entry(entity).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
        }
    }

    public virtual async Task DeleteAsync(TEntity entity)
    {
        entity.MarkAsDeleted();
        await UpdateAsync(entity);
    }

    public virtual async Task<IEnumerable<TEntity>> RetrieveAllAsync()
    {
        return await _dbContext.Set<TEntity>()
            .Where(entity => entity.IsDeleted == false)
            .ToListAsync();
    }

    # pragma warning disable CS8603
    public virtual async Task<TEntity> RetrieveByIdAsync(int id)
    {
        return await _dbContext.Set<TEntity>()
            .Where(entity => entity.IsDeleted == false)
            .FirstOrDefaultAsync(entity => entity.Id == id);
    }
}