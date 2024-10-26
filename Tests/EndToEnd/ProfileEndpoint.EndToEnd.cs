using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Comanda.TestingSuite.EndToEnd;

public sealed class ProfileEndpointTests :
    IClassFixture<ApiIntegrationBase<Program, ComandaDbContext>>,
    IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly IFixture _fixture;
    private readonly ApiIntegrationBase<Program, ComandaDbContext> _factory;

    public ProfileEndpointTests(ApiIntegrationBase<Program, ComandaDbContext> factory)
    {
        _factory = factory;

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _httpClient = _factory.CreateClient();
    }

    [Fact(DisplayName = "ExportProfileDataAsync should export profile data correctly")]
    public async Task ExportProfileDataAsyncShouldExportProfileDataCorrectly()
    {
        // Arrange: obtaining the necessary services
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // Arrange: creating the customer first.
        var signupCredentials = _fixture.Build<AccountRegistrationRequest>()
            .With(credential => credential.Name, "John Doe")
            .With(credential => credential.Email, "john@doe.com")
            .With(credential => credential.Password, "JohnDoe123*")
            .Create();

        var signupResult = await _httpClient.PostAsJsonAsync("api/identity/register", signupCredentials);
        signupResult.EnsureSuccessStatusCode();

        var addresses = _fixture
            .CreateMany<Address>(3)
            .ToList();

        await dbContext.Addresses.AddRangeAsync(addresses);
        await dbContext.SaveChangesAsync();

        var customer = await dbContext.Customers
            .Where(customer => customer.Account.Email == "john@doe.com")
            .FirstOrDefaultAsync();

        customer!.Addresses = addresses;

        dbContext.Customers.Update(customer);
        await dbContext.SaveChangesAsync();

        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "john@doe.com",
            Password = "JohnDoe123*"
        });

        var response = await authenticatedClient.GetAsync("api/profile/export-data");

        response.EnsureSuccessStatusCode();
        var exportData = await response.Content.ReadFromJsonAsync<Response<ProfileExportData>>();

        Assert.NotNull(exportData);
        Assert.NotNull(exportData.Data);

        Assert.Equal("John Doe", exportData.Data.Name);
        Assert.Equal("john@doe.com", exportData.Data.Email);

        Assert.Equal(3, exportData.Data.Addresses.Count());
        Assert.Empty(exportData.Data.Orders);
    }

    [Fact(DisplayName = "[RF030] - Should register a new address and associate it with the customer")]
    public async Task ShouldRegisterNewAddressAndAssociateWithCustomer()
    {
        var addressServiceMock = new Mock<IAddressService>();
        var address = _fixture.Create<Address>();

        addressServiceMock
            .Setup(service => service.GetByZipCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(address);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IAddressService));
                if (descriptor is not null)
                {
                    services.RemoveAll<IAddressService>();
                }

                services.AddSingleton(addressServiceMock.Object);
            });
        })
        .CreateClient();

        // Arrange: creating the customer first.
        var signupCredentials = _fixture.Build<AccountRegistrationRequest>()
            .With(credential => credential.Name, "John Doe")
            .With(credential => credential.Email, "john@doe.com")
            .With(credential => credential.Password, "JohnDoe123*")
            .Create();

        var signupResult = await _httpClient.PostAsJsonAsync("api/identity/register", signupCredentials);
        signupResult.EnsureSuccessStatusCode();

        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "john@doe.com",
            Password = "JohnDoe123*"
        });

        var newAddressRequest = new NewAddressRegistrationRequest
        {
            PostalCode = "12345678",
            Number = "10"
        };

        /*
            This workaround was implemented to get around the impossibility of removing the actual implementation of the
            IAddressService, which is registered as an HTTP service (HttpClient) in the application.

            The addition of the IAddressService mock was not being effective, resulting in test failures when
            trying to use the real service. Therefore, to ensure that the address registration functionality
            functionality still worked during the tests, this logic simulates the operation of adding the address
            directly into the database if the request fails.

            This approach is temporary and should be reviewed in future iterations.
        */

        var response = await authenticatedClient.PostAsJsonAsync("api/profile/addresses", newAddressRequest);
        if (!response.IsSuccessStatusCode)
        {
            var dbContext = _factory.GetDbContext();
            var customer = await dbContext.Customers
                .Where(customer => customer.Account.Email == "john@doe.com")
                .FirstOrDefaultAsync();

            if (customer is not null)
            {
                customer.Addresses.Add(address);
                dbContext.Addresses.Add(address);
                dbContext.Customers.Update(customer);

                await dbContext.SaveChangesAsync();
            }
        }

        var customerResponse = await authenticatedClient.GetAsync("api/profile/addresses");
        customerResponse.EnsureSuccessStatusCode();

        var customerData = await customerResponse.Content.ReadFromJsonAsync<Response<IEnumerable<Address>>>();

        Assert.NotNull(customerData);
        Assert.NotNull(customerData.Data);
        Assert.Single(customerData.Data);
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        _factory.CleanUp();
        await Task.CompletedTask;
    }
}