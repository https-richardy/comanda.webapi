namespace Comanda.TestingSuite.Fixtures;

public abstract class WebApiFixture : IClassFixture<WebApiFactoryFixture<Program>>, IAsyncLifetime
{
    protected readonly HttpClient HttpClient;
    protected readonly WebApiFactoryFixture<Program> Factory;
    protected readonly IFixture Fixture;

    public WebApiFixture(WebApiFactoryFixture<Program> factory)
    {
        HttpClient = factory.CreateClient();
        Factory = factory;

        Fixture = new Fixture();
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
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