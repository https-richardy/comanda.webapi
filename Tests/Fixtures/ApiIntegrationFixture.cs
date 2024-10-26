using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Hosting;

namespace Comanda.TestingSuite.Fixtures;

public class ApiIntegrationBase<TStartup, TDbContext> : WebApplicationFactory<TStartup>, IDisposable
    where TStartup : class
    where TDbContext : DbContext
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<TDbContext> _contextOptions;
    private IServiceScope? _scope;
    private bool _disposed;

    public ApiIntegrationBase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<TDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = CreateDbContext();
        context.Database.EnsureCreated();
    }

    protected virtual TDbContext CreateDbContext()
    {
        return (Activator.CreateInstance(typeof(TDbContext), _contextOptions) as TDbContext)!;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                descriptor => descriptor.ServiceType == typeof(DbContextOptions<TDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<TDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            services.AddScoped(provider =>
            {
                return CreateDbContext();
            });
        });

        var host = base.CreateHost(builder);
        _scope = host.Services.CreateScope();

        return host;
    }

    public TDbContext GetDbContext()
    {
        EnsureScope();
        return _scope!.ServiceProvider.GetRequiredService<TDbContext>();
    }

    public IServiceProvider GetServiceProvider()
    {
        EnsureScope();
        return _scope!.ServiceProvider;
    }

    private void EnsureScope()
    {
        /*
            This method is used to ensure that a service scope is available for resolving services.
            It is idempotent, meaning that if the scope is already created, it will not be recreated.
        */

        if (_scope is null)
            _scope = Services.CreateScope();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _scope?.Dispose();
                _connection?.Dispose();
            }

            _disposed = true;
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Ensures the test database is clean before running each test.
    /// </summary>
    /// <remarks>
    /// This method is idempotent, meaning that if the database already exists, it will be deleted and recreated.
    /// </remarks>
    public void CleanUp()
    {
        using var context = CreateDbContext();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        _scope?.Dispose();
        _scope = Services.CreateScope();
    }

    public async Task<HttpClient> AuthenticateClientAsync(AuthenticationCredentials credentials)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("api/identity/authenticate", credentials);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Authentication failed with status code: {response.StatusCode}. " +
                $"Response: {await response.Content.ReadAsStringAsync()}"
            );
        }

        var authenticationResult = await response.Content.ReadFromJsonAsync<Response<AuthenticationResponse>>();

        if (authenticationResult?.Data?.Token is null)
            throw new InvalidOperationException("Authentication response did not contain a token");

        var authenticationHeader = new AuthenticationHeaderValue("Bearer", authenticationResult.Data.Token);
        client.DefaultRequestHeaders.Authorization = authenticationHeader;

        return client;
    }
}