namespace Comanda.TestingSuite.Fixtures;

public abstract class WebApiFixture<TDbContext> :
    IClassFixture<WebApiFactoryFixture<Program>>, IAsyncLifetime
    where TDbContext : DbContext
{
    protected readonly HttpClient HttpClient;
    protected TDbContext DbContext = default!;
    protected readonly WebApiFactoryFixture<Program> Factory;
    protected readonly IFixture Fixture;

    protected string _bearerToken = string.Empty;
    protected HttpClient _authenticatedClient = null!;


    public WebApiFixture(WebApiFactoryFixture<Program> factory)
    {
        HttpClient = factory.CreateClient();
        Factory = factory;

        Fixture = new Fixture();
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public virtual async Task DisposeAsync()
    {
        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
            DbContext = context;

            await DbContext.Database.EnsureDeletedAsync();
            await context.Database.EnsureDeletedAsync();
        }
    }

    public virtual async Task InitializeAsync()
    {
        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
            DbContext = context;

            await DbContext.Database.EnsureCreatedAsync();
            await context.Database.EnsureCreatedAsync();
        }
    }

    protected async Task AuthenticateAdminUserAsync()
    {
        var payload = new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "Ri34067294*"
        };

        var response = await HttpClient.PostAsJsonAsync("api/identity/authenticate", payload);
        var responseContent = await response.Content.ReadFromJsonAsync<Response<AuthenticationResponse>>();

        response.EnsureSuccessStatusCode();
        _bearerToken = responseContent!.Data!.Token ?? throw new Exception("authentication failed");

        _authenticatedClient = Factory.CreateClient();
        _authenticatedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
    }

    protected HttpClient GetAuthenticatedClient()
    {
        if (_authenticatedClient is null)
            throw new Exception("Authenticated client not initialized.");

        return _authenticatedClient;
    }
}
