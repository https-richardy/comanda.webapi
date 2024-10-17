namespace Comanda.TestingSuite.Fixtures;

public abstract class SqliteDatabaseFixture<TDbContext> : IAsyncLifetime
    where TDbContext : DbContext
{
    protected IFixture Fixture { get; private set; }
    protected TDbContext DbContext { get; private set; }

    protected SqliteDatabaseFixture()
    {
        var connectionString = $"Data Source={Path.GetTempFileName()};";
        var options = new DbContextOptionsBuilder<TDbContext>()
            .UseSqlite(connectionString)
            .Options;

        DbContext = (Activator.CreateInstance(typeof(TDbContext), options) as TDbContext)!;

        Fixture = new Fixture();
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public async Task InitializeAsync()
    {
        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.DisposeAsync();
    }
}