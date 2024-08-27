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
}