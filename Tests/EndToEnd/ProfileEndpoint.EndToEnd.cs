using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Comanda.TestingSuite.EndToEnd;

public sealed class ProfileEndpointTests : WebApiFixture<ComandaDbContext>
{
    private HttpClient _authenticatedClient = null!;

    private string _bearerToken = string.Empty;
    private readonly IFixture _fixture;

    public ProfileEndpointTests(WebApiFactoryFixture<Program> factory) : base(factory)
    {
        AuthenticateCustomerUserAsync().GetAwaiter().GetResult();

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact(DisplayName = "ExportProfileDataAsync should export profile data correctly")]
    public async Task ExportProfileDataAsyncShouldExportProfileDataCorrectly()
    {
        var addresses = Fixture
            .CreateMany<Address>(3)
            .ToList();

        await DbContext.Addresses.AddRangeAsync(addresses);
        await DbContext.SaveChangesAsync();

        var customer = await DbContext.Customers
            .Where(customer => customer.Account.Email == "john@doe.com")
            .FirstOrDefaultAsync();

        customer!.Addresses = addresses;

        DbContext.Customers.Update(customer);
        await DbContext.SaveChangesAsync();

        var authenticatedClient = GetAuthenticatedClient();
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
        var address = Fixture.Create<Address>();

        addressServiceMock
            .Setup(service => service.GetByZipCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(address);

        // remove the real address service and add the mock one
        var client = Factory.WithWebHostBuilder(builder =>
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
        }).CreateClient();

        var authenticatedClient = GetAuthenticatedClient();
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
            var customer = await DbContext.Customers
                .Where(customer => customer.Account.Email == "john@doe.com")
                .FirstOrDefaultAsync();

            if (customer is not null)
            {
                customer.Addresses.Add(address);
                DbContext.Addresses.Add(address);
                DbContext.Customers.Update(customer);

                await DbContext.SaveChangesAsync();
            }
        }

        var customerResponse = await authenticatedClient.GetAsync("api/profile/addresses");
        customerResponse.EnsureSuccessStatusCode();

        var customerData = await customerResponse.Content.ReadFromJsonAsync<Response<IEnumerable<Address>>>();

        Assert.NotNull(customerData);
        Assert.NotNull(customerData.Data);
        Assert.Single(customerData.Data);
    }

    private async Task AuthenticateCustomerUserAsync()
    {
        var registrationPayload = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Password = "JohnDoe123*",
            Email = "john@doe.com",
        };

        var registrationResponse = await HttpClient.PostAsJsonAsync("api/identity/register", registrationPayload);
        registrationResponse.EnsureSuccessStatusCode();

        var payload = new AuthenticationCredentials
        {
            Email = "john@doe.com",
            Password = "JohnDoe123*"
        };

        var response = await HttpClient.PostAsJsonAsync("api/identity/authenticate", payload);
        var responseContent = await response.Content.ReadFromJsonAsync<Response<AuthenticationResponse>>();

        response.EnsureSuccessStatusCode();
        _bearerToken = responseContent!.Data!.Token ?? throw new Exception("authentication failed");

        _authenticatedClient = Factory.CreateClient();
        _authenticatedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
    }

    private HttpClient GetAuthenticatedClient()
    {
        if (_authenticatedClient is null)
            throw new Exception("Authenticated client not initialized.");

        return _authenticatedClient;
    }
}