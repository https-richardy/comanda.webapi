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

    [Fact(DisplayName = "Should retrieve a category by ID successfully")]
    public async Task ShouldRetrieveCategoryByIdSuccessfully()
    {
        var category = new Category { Name = "Category to Retrieve" };
        await _repository.SaveAsync(category);

        var retrievedCategory = await _repository.RetrieveByIdAsync(category.Id);

        Assert.NotNull(retrievedCategory);
        Assert.Equal(category.Name, retrievedCategory.Name);
    }

    [Fact(DisplayName = "Should retrieve all categories")]
    public async Task ShouldRetrieveAllCategories()
    {
        var categories = new List<Category>
        {
            new Category { Name = "Food" },
            new Category { Name = "Drinks" }
        };

        await DbContext.Categories.AddRangeAsync(categories);
        await DbContext.SaveChangesAsync();

        var allCategories = await _repository.RetrieveAllAsync();

        Assert.Equal(2, allCategories.Count());

        Assert.Contains(categories[0], allCategories);
        Assert.Contains(categories[1], allCategories);
    }

    [Fact(DisplayName = "Should find a single category by predicate")]
    public async Task ShouldFindSingleCategoryByPredicate()
    {
        var category = new Category { Name = "Special Category" };
        await _repository.SaveAsync(category);

        var foundCategory = await _repository.FindSingleAsync(category => category.Name == "Special Category");

        Assert.NotNull(foundCategory);
        Assert.Equal(category.Name, foundCategory.Name);
    }

    [Fact(DisplayName = "Should find all categories by predicate")]
    public async Task ShouldFindAllCategoriesByPredicate()
    {
        var categories = new List<Category>
        {
            new Category { Name = "Combos" },
            new Category { Name = "Classic" },
            new Category { Name = "Drinks" }
        };

        await DbContext.Categories.AddRangeAsync(categories);
        await DbContext.SaveChangesAsync();

        var foundCategories = await _repository.FindAllAsync(category => category.Name.StartsWith('C'));

        Assert.Equal(2, foundCategories.Count());
        Assert.Contains(categories[0], foundCategories);
        Assert.Contains(categories[1], foundCategories);
    }

    [Fact(DisplayName = "Should retrieve paged categories")]
    public async Task ShouldRetrievePagedCategories()
    {
        var categories = new List<Category>
        {
            new Category { Name = "Category 1" },
            new Category { Name = "Category 2" },
            new Category { Name = "Category 3" },
            new Category { Name = "Category 4" },
            new Category { Name = "Category 5" },
            new Category { Name = "Category 6" }
        };

        await DbContext.Categories.AddRangeAsync(categories);
        await DbContext.SaveChangesAsync();

        const int pageNumber = 1;
        const int pageSize = 3;

        var pagedCategories = await _repository.PagedAsync(pageNumber, pageSize);

        Assert.Equal(3, pagedCategories.Count());
        Assert.Contains(categories[0], pagedCategories);
        Assert.Contains(categories[1], pagedCategories);
        Assert.Contains(categories[2], pagedCategories);
    }

    [Fact(DisplayName = "Should retrieve paged categories by predicate")]
    public async Task ShouldRetrievePagedCategoriesByPredicate()
    {
        var categories = new List<Category>
        {
            new Category { Name = "Category A" },
            new Category { Name = "Category B" },
            new Category { Name = "Category C" },
            new Category { Name = "Category D" },
            new Category { Name = "Category E" }
        };

        await DbContext.Categories.AddRangeAsync(categories);
        await DbContext.SaveChangesAsync();

        const int pageNumber = 1;
        const int pageSize = 2;

        Expression<Func<Category, bool>> predicate = category => category.Name.StartsWith('C');

        var pagedCategories = await _repository.PagedAsync(predicate, pageNumber, pageSize);

        Assert.Equal(2, pagedCategories.Count());
        Assert.Contains(categories[0], pagedCategories);
        Assert.Contains(categories[1], pagedCategories);
    }

    [Fact(DisplayName = "Should check if a category exists")]
    public async Task ShouldCheckIfCategoryExists()
    {
        var category = new Category { Name = "Category Exists" };
        await _repository.SaveAsync(category);

        var exists = await _repository.ExistsAsync(category.Id);
        Assert.True(exists);
    }
}