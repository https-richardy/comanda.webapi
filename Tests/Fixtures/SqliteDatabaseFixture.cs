using Microsoft.Data.Sqlite;

namespace Comanda.TestingSuite.Fixtures;

public abstract class SqliteDatabaseFixture<TDbContext> : IAsyncLifetime
    where TDbContext : DbContext
{
    private readonly SqliteConnection _connection;

    protected IFixture Fixture { get; private set; }
    protected TDbContext DbContext { get; private set; }

    protected SqliteDatabaseFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<TDbContext>()
            .UseSqlite(_connection)
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

        await _connection.DisposeAsync();
    }
}