namespace Comanda.TestingSuite.UnitTests.Repositories;

public sealed class CategoryRepositoryTests : InMemoryDatabaseFixture<ComandaDbContext>
{
    private readonly ICategoryRepository _repository;

    public CategoryRepositoryTests()
    {
        _repository = new CategoryRepository(dbContext: DbContext);
    }

    [Fact(DisplayName = "Given a new category, should save successfully in the database")]
    public async Task GivenNewCategory_ShouldSaveSuccessfullyInTheDatabase()
    {
        var category = new Category { Name = "New Category" };

        await _repository.SaveAsync(category);
        var savedCategory = await DbContext.Categories.FindAsync(category.Id);

        Assert.NotNull(savedCategory);
        Assert.Equal(category.Name, savedCategory.Name);
    }

    [Fact(DisplayName = "Given an existing category, should delete it successfully")]
    public async Task GivenExistingCategory_ShouldDeleteSuccessfully()
    {
        var category = new Category { Name = "Category to Delete" };

        await _repository.SaveAsync(category);
        await _repository.DeleteAsync(category);

        var deletedCategory = await DbContext.Categories.FindAsync(category.Id);

        Assert.Null(deletedCategory);
    }

    [Fact(DisplayName = "Given an updated category, should update it successfully")]
    public async Task GivenUpdatedCategory_ShouldUpdateSuccessfully()
    {
        var category = new Category { Name = "Old Name" };
        await _repository.SaveAsync(category);

        category.Name = "Updated Name";
        await _repository.UpdateAsync(category);

        var updatedCategory = await DbContext.Categories.FindAsync(category.Id);

        Assert.Equal("Updated Name", updatedCategory!.Name);
    }
}