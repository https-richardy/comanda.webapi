using Microsoft.Extensions.Configuration;

namespace Comanda.TestingSuite.Fixtures;

/// <summary>
/// Provides a base class for integration tests, setting up an in-memory database and a service provider.
/// This class is responsible for setting up the necessary dependencies, including a new in-memory database, 
/// and tearing it down after each test execution. It ensures that each test has a clean state to work with.
/// </summary>
/// <remarks>
/// The <see cref="IntegrationFixture{TDbContext}"/> class is designed to facilitate integration testing by 
/// simulating real interactions between system components, including a database in a controlled environment. 
/// It uses an in-memory database, which is created and destroyed for each test to ensure that tests are isolated 
/// and not affected by state from other tests.
/// The fixture also mocks essential services like configuration settings to provide a complete integration environment.
/// </remarks>
/// <typeparam name="TDbContext">The type of the DbContext that the tests will use, typically the application's primary DbContext.</typeparam>
public abstract class IntegrationFixture<TDbContext> : IAsyncLifetime
    where TDbContext : DbContext
{
    protected IFixture Fixture { get; private set; }
    protected IServiceCollection Services { get; private set; }
    protected IServiceProvider ServiceProvider { get; private set; } = default!;
    protected TDbContext DbContext { get; private set; } = default!;

    public IntegrationFixture()
    {
        Fixture = new Fixture();
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        Services = new ServiceCollection();

        var dbContextOptionsDescriptor = Services.SingleOrDefault(descriptor => descriptor.ServiceType == typeof(DbContextOptions<TDbContext>));
        if (dbContextOptionsDescriptor is not null)
            Services.Remove(dbContextOptionsDescriptor);

        Services.AddDbContext<TDbContext>(options =>
        {
            var connectionString = $"Data Source={Path.GetTempFileName()};";
            options.UseSqlite(connectionString);
        });

        var inMemorySettings = new Dictionary<string, string>
        {
            { "Stripe:SecretKey", "sk_test_mocked_stripe_key" },
            { "Gemini:ApiKey", "mocked.gemini.api.key" },
            { "JwtSettings:SecretKey", Guid.NewGuid().ToString() },
            { "SmtpSettings:Host", "mocked.smtp.host" },
            { "SmtpSettings:UserName", "mocked.smtp.user" },
            { "SmtpSettings:Password", "mocked.smtp.password" },
            { "SmtpSettings:Port", "587" },
            { "SmtpSettings:UseSsl", "true" },
            { "ExternalApis:ViaCep", "https://mocked.viacep.api.com/ws" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        Services.AddLogging();

        Services.AddSingleton<IConfiguration>(configuration);
        Services.ConfigureServices(configuration);
    }

    public async Task DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        ServiceProvider = Services.BuildServiceProvider();
        DbContext = ServiceProvider.GetRequiredService<TDbContext>();

        await DbContext.Database.EnsureCreatedAsync();
    }
}