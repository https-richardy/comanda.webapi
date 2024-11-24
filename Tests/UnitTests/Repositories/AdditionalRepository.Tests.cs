namespace Comanda.TestingSuite.UnitTests.Repositories;

public sealed class AdditionalRepositoryTests : SqliteDatabaseFixture<ComandaDbContext>
{
    private readonly IAdditionalRepository _repository;

    public AdditionalRepositoryTests()
    {
        _repository = new AdditionalRepository(DbContext);
    }

    [Fact(DisplayName = "Given a new additional, should save successfully in the database")]
    public async Task GivenNewAdditional_ShouldSaveSuccessfullyInTheDatabase()
    {
        var additional = Fixture.Create<Additional>();

        await _repository.SaveAsync(additional);
        var savedAdditional = await DbContext.Additionals.FindAsync(additional.Id);

        Assert.NotNull(savedAdditional);
        Assert.Equal(additional.Id, savedAdditional.Id);
        Assert.Equal(additional.Name, savedAdditional.Name);
        Assert.Equal(additional.Price, savedAdditional.Price);
        Assert.Equal(additional.Category.Id, savedAdditional.Category.Id);
    }

    [Fact(DisplayName = "Given a valid additional, should update successfully in the database")]
    public async Task GivenValidAdditional_ShouldUpdateSuccessfullyInTheDatabase()
    {
        var additional = Fixture.Create<Additional>();

        await DbContext.Additionals.AddAsync(additional);
        await DbContext.SaveChangesAsync();

        additional.Name = "Updated Name";
        additional.Price = 19.99m;

        await _repository.UpdateAsync(additional);
        var updatedAdditional = await DbContext.Additionals.FindAsync(additional.Id);

        Assert.NotNull(updatedAdditional);
        Assert.Equal(additional.Id, updatedAdditional.Id);
        Assert.Equal(additional.Name, updatedAdditional.Name);
        Assert.Equal(additional.Price, updatedAdditional.Price);
    }

    [Fact(DisplayName = "Given a valid additional, should delete successfully from the database")]
    public async Task GivenValidAdditional_ShouldDeleteSuccessfullyFromTheDatabase()
    {
        var additional = Fixture.Create<Additional>();

        await DbContext.Additionals.AddAsync(additional);
        await DbContext.SaveChangesAsync();

        await _repository.DeleteAsync(additional);
        var deletedAdditional = await DbContext.Additionals.FindAsync(additional.Id);

        Assert.NotNull(deletedAdditional);
        Assert.True(deletedAdditional.IsDeleted);
    }

    [Fact(DisplayName = "Should fetch all additionals")]
    public async Task ShouldFetchAllAdditionals()
    {
        var additionals = Fixture.CreateMany<Additional>(5).ToList();

        await DbContext.Additionals.AddRangeAsync(additionals);
        await DbContext.SaveChangesAsync();

        var foundAdditionals = await _repository.RetrieveAllAsync();

        Assert.Equal(additionals.Count, foundAdditionals.Count());
    }

    [Fact(DisplayName = "Given an existing additional ID, should return true")]
    public async Task GivenExistingAdditionalId_ShouldReturnTrue()
    {
        var additional = Fixture.Create<Additional>();

        await DbContext.Additionals.AddAsync(additional);
        await DbContext.SaveChangesAsync();

        var exists = await _repository.ExistsAsync(additional.Id);
        Assert.True(exists);
    }

    [Fact(DisplayName = "Should count all additionals")]
    public async Task ShouldCountAllAdditionals()
    {
        var additionals = Fixture.CreateMany<Additional>(10).ToList();

        await DbContext.Additionals.AddRangeAsync(additionals);
        await DbContext.SaveChangesAsync();

        var count = await _repository.CountAsync(_ => true);
        Assert.Equal(additionals.Count, count);
    }

    [Fact(DisplayName = "Given a valid predicate, should count matching additionals")]
    public async Task GivenPredicate_ShouldCountMatchingAdditionals()
    {
        var additionals = Fixture.CreateMany<Additional>(10).ToList();
        var categoryToSearch = "Category B";

        additionals[0].Name = categoryToSearch;
        additionals[1].Name = categoryToSearch;

        await DbContext.Additionals.AddRangeAsync(additionals);
        await DbContext.SaveChangesAsync();

        var count = await _repository.CountAsync(additional => additional.Name == categoryToSearch);
        Assert.Equal(2, count);
    }

    [Fact(DisplayName = "Should fetch additionals in pages")]
    public async Task ShouldFetchAdditionalsInPages()
    {
        var additionals = Fixture.CreateMany<Additional>(10).ToList();

        await DbContext.Additionals.AddRangeAsync(additionals);
        await DbContext.SaveChangesAsync();

        const int pageNumber = 1;
        const int pageSize = 5;

        var pagedAdditionals = await _repository.PagedAsync(pageNumber, pageSize);

        Assert.Equal(pageSize, pagedAdditionals.Count());
        Assert.Contains(additionals, additional => pagedAdditionals.Any(paged => paged.Id == additional.Id));
    }

    [Fact(DisplayName = "Given a valid predicate, should fetch additionals in pages")]
    public async Task GivenPredicate_ShouldFetchAdditionalsInPages()
    {
        var drinkCategory = new Category { Name = "Drinks" };
        var foodCategory = new Category { Name = "Food" };

        var additional1 = new Additional { Name = "Soda", Price = 1.99m, Category = drinkCategory };
        var additional2 = new Additional { Name = "Water", Price = 0.99m, Category = drinkCategory };
        var additional3 = new Additional { Name = "Burger", Price = 5.99m, Category = foodCategory };
        var additional4 = new Additional { Name = "Fries", Price = 2.99m, Category = foodCategory };
        var additional5 = new Additional { Name = "Pizza", Price = 7.99m, Category = foodCategory };
        var additional6 = new Additional { Name = "Pasta", Price = 6.99m, Category = foodCategory };

        var additionals = new List<Additional> { additional1, additional2, additional3, additional4, additional5, additional6 };
        var categories = new List<Category> { drinkCategory, foodCategory };

        await DbContext.Categories.AddRangeAsync(categories);
        await DbContext.Additionals.AddRangeAsync(additionals);
        await DbContext.SaveChangesAsync();

        const int pageNumber = 1;
        const int pageSize = 5;

        Expression<Func<Additional, bool>> predicate = additional => additional.Category.Name == "Food";

        var pagedAdditionals = await _repository.PagedAsync(predicate, pageNumber, pageSize);

        Assert.Equal(4, pagedAdditionals.Count());
        Assert.Contains(additional3, pagedAdditionals);
        Assert.Contains(additional4, pagedAdditionals);
        Assert.Contains(additional5, pagedAdditionals);
        Assert.Contains(additional6, pagedAdditionals);
    }
}