namespace Comanda.TestingSuite.UnitTests.Repositories;

public sealed class AdditionalRepositoryTests : InMemoryDatabaseFixture<ComandaDbContext>
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

        Assert.Null(deletedAdditional);
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
}