
namespace Comanda.WebApi.TestingSuite.Repositories;

public sealed class EstablishmentRepositoryTestingSuite : IAsyncLifetime
{
    private readonly ComandaDbContext _dbContext;
    private readonly EstablishmentRepository _repository;
    private readonly IFixture _fixture;

    public EstablishmentRepositoryTestingSuite()
    {
        var optionsBuilder = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());

        _dbContext = new ComandaDbContext(optionsBuilder.Options);
        _repository = new EstablishmentRepository(_dbContext);

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact(DisplayName = "FindOwnerAsync - Should return establishment owner and related properties")]
    public async Task FindOwnerAsync_ShouldReturnEstablishmentOwnerAndRelatedProperties()
    {
        var establishment = _fixture.Create<Establishment>();

        await _dbContext.Establishments.AddAsync(establishment);
        await _dbContext.SaveChangesAsync();

        var owner = await _repository.FindOwnerAsync(establishment.Id);

        Assert.NotNull(owner);
        Assert.NotNull(owner.Account);
        Assert.Equal(establishment.Owner, owner);
        Assert.Equal(establishment.Owner.Account.Id, owner.Account.Id);
        Assert.Equal(establishment.Owner.Account.UserName, owner.Account.UserName);
    }


    public async Task InitializeAsync()
    {
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        _dbContext.Dispose();
    }
}