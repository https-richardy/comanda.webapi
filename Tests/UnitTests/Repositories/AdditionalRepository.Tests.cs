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
}