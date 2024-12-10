namespace Comanda.WebApi.Services;

public sealed class CategoryManager : ICategoryManager
{
    public IQueryable<Category> Categories => _categoryRepository.Entities;
    private readonly ICategoryRepository _categoryRepository;

    public CategoryManager(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task CreateAsync(Category category)
    {
        await _categoryRepository.SaveAsync(category);
    }

    public async Task DeleteAsync(Category category)
    {
        await _categoryRepository.DeleteAsync(category);
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _categoryRepository.RetrieveAllAsync();
    }

    public async Task<Category?> GetAsync(int id)
    {
        return await _categoryRepository.RetrieveByIdAsync(id);
    }

    public async Task UpdateAsync(Category category)
    {
        await _categoryRepository.UpdateAsync(category);
    }
}