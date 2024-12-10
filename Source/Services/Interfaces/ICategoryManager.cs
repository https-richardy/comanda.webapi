namespace Comanda.WebApi.Services;

public interface ICategoryManager
{
    IQueryable<Category> Categories { get; }

    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetAsync(int id);
    Task CreateAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Category category);
}