using AutoFixture;
using Microsoft.EntityFrameworkCore;

namespace Comanda.TestingSuite.Fixtures;

/// <summary>
/// An abstract base class for setting up an in-memory SQLite database fixture for testing.
/// This class configures an in-memory SQLite database and provides an instance of <typeparamref name="TDbContext"/> 
/// to be used in tests. It also initializes AutoFixture for creating test data.
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public abstract class SqliteDatabaseFixture<TDbContext> : IAsyncLifetime
    where TDbContext : DbContext
{
    protected IFixture Fixture { get; private set; }
    protected TDbContext DbContext { get; private set; }

    protected SqliteDatabaseFixture()
    {
        const string connectionString = "Data Source=:memory:";
        var options = new DbContextOptionsBuilder<TDbContext>()
            .UseSqlite(connectionString)
            .Options;

        DbContext = (Activator.CreateInstance(typeof(TDbContext), options) as TDbContext)!;

        Fixture = new Fixture();
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public async Task DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await DbContext.Database.EnsureCreatedAsync();
    }
}