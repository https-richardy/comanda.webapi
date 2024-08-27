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
}