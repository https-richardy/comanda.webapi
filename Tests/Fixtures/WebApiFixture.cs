namespace Comanda.TestingSuite.Fixtures;

public abstract class WebApiFixture<TDbContext> :
    IClassFixture<WebApiFactoryFixture<Program>>, IAsyncLifetime
    where TDbContext : DbContext
{
    protected readonly HttpClient HttpClient;
    protected readonly TDbContext DbContext;
    protected readonly WebApiFactoryFixture<Program> Factory;
    protected readonly IFixture Fixture;

    public WebApiFixture(WebApiFactoryFixture<Program> factory)
    {
        HttpClient = factory.CreateClient();
        Factory = factory;

        Fixture = new Fixture();
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        using (var scope = factory.Services.CreateScope())
        {
            DbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        }
    }

    public virtual async Task DisposeAsync()
    {
        using (var scope = Factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ComandaDbContext>();
            await context.Database.EnsureDeletedAsync();
        }
    }

    public virtual async Task InitializeAsync()
    {
        using (var scope = Factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ComandaDbContext>();
            await context.Database.EnsureCreatedAsync();
        }
    }
}