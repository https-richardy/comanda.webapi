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
        var address = _fixture.Build<Address>()
            .Without(address => address.Reference)
            .Without(address => address.Complement)
            .Create();

        addressServiceMock
            .Setup(service => service.GetByZipCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(address);

        var scopedClient = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IAddressService));
                if (descriptor is not null)
                {
                    services.RemoveAll<IAddressService>();
                }

                services.AddScoped(provider => addressServiceMock.Object);
            });
        })
        .CreateClient();

        // arrange: creating the customer first.
        var signupCredentials = _fixture.Build<AccountRegistrationRequest>()
            .With(credential => credential.Name, "John Doe")
            .With(credential => credential.Email, "john.doe@email.com")
            .With(credential => credential.Password, "JohnDoe1234*")
            .Create();

        var signupResult = await scopedClient.PostAsJsonAsync("api/identity/register", signupCredentials);
        signupResult.EnsureSuccessStatusCode();

        // arrange: authenticate httpClient as customer

        var authenticationResponse = await scopedClient.PostAsJsonAsync($"api/identity/authenticate", new AuthenticationCredentials
        {
            Email = "john.doe@email.com",
            Password = "JohnDoe1234*"
        });

        var authenticationContent = await authenticationResponse.Content.ReadFromJsonAsync<Response<AuthenticationResponse>>();
        authenticationResponse.EnsureSuccessStatusCode();

        Assert.NotNull(authenticationContent);
        Assert.NotNull(authenticationContent.Data);
        Assert.NotNull(authenticationContent.Data.Token);

        var authorizationHeader = new AuthenticationHeaderValue("Bearer", authenticationContent.Data.Token);
        scopedClient.DefaultRequestHeaders.Authorization = authorizationHeader;

        var newAddressRequest = new NewAddressRegistrationRequest
        {
            PostalCode = "12345678",
            Number = "10",
            Complement = _fixture.Create<string>(),
            Reference = _fixture.Create<string>()
        };

        var response = await scopedClient.PostAsJsonAsync("api/profile/addresses", newAddressRequest);
        var customerResponse = await scopedClient.GetAsync("api/profile/addresses");

        customerResponse.EnsureSuccessStatusCode();
        var customerAddresses = await customerResponse.Content.ReadFromJsonAsync<Response<IEnumerable<Address>>>();

        Assert.NotNull(customerAddresses);
        Assert.NotNull(customerAddresses.Data);
        Assert.Single(customerAddresses.Data);

        var registeredAddress = customerAddresses.Data.First();

        Assert.NotNull(registeredAddress.Complement);
        Assert.NotNull(registeredAddress.Reference);

        Assert.False(string.IsNullOrEmpty(registeredAddress.Complement));
        Assert.False(string.IsNullOrEmpty(registeredAddress.Reference));
    }

    [Fact(DisplayName = "Should update an existing address and return the updated information")]
    public async Task ShouldUpdateExistingAddressAndReturnUpdatedInfo()
    {
        // arrange: obtaining the necessary services and mocking the address service
        var addressServiceMock = new Mock<IAddressService>();
        var address = _fixture.Build<Address>()
            .Without(address => address.Reference)
            .Without(address => address.Complement)
            .Create();

        addressServiceMock
            .Setup(service => service.GetByZipCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(address);

        // arrange: obtaining the necessary services
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: creating the customer first.
        var signupCredentials = _fixture.Build<AccountRegistrationRequest>()
            .With(credential => credential.Name, "Jane Doe")
            .With(credential => credential.Email, "jane@doe.com")
            .With(credential => credential.Password, "JaneDoe123*")
            .Create();

        // remove the real address service and add the mock
        var scopedClient = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IAddressService));
                if (descriptor is not null)
                {
                    services.RemoveAll<IAddressService>();
                }

                services.AddScoped(provider => addressServiceMock.Object);
            });
        })
        .CreateClient();

        var signupResult = await scopedClient.PostAsJsonAsync("api/identity/register", signupCredentials);
        signupResult.EnsureSuccessStatusCode();

        // arrange: authenticate httpClient (scoped) as customer
        var authenticationResponse = await scopedClient.PostAsJsonAsync($"api/identity/authenticate", new AuthenticationCredentials
        {
            Email = "jane@doe.com",
            Password = "JaneDoe123*"
        });

        var authenticationContent = await authenticationResponse.Content.ReadFromJsonAsync<Response<AuthenticationResponse>>();
        authenticationResponse.EnsureSuccessStatusCode();

        Assert.NotNull(authenticationContent);
        Assert.NotNull(authenticationContent.Data);
        Assert.NotNull(authenticationContent.Data.Token);

        var authorizationHeader = new AuthenticationHeaderValue("Bearer", authenticationContent.Data.Token);
        scopedClient.DefaultRequestHeaders.Authorization = authorizationHeader;

        // arrange: create a sample address
        var newAddressRequest = new NewAddressRegistrationRequest
        {
            PostalCode = "12345678",
            Number = "10",
            Complement = _fixture.Create<string>(),
            Reference = _fixture.Create<string>()
        };

        var response = await scopedClient.PostAsJsonAsync("api/profile/addresses", newAddressRequest);
        var customerAddressesResponse = await scopedClient.GetAsync("api/profile/addresses");

        customerAddressesResponse.EnsureSuccessStatusCode();
        var customerAddresses = await customerAddressesResponse.Content.ReadFromJsonAsync<Response<IEnumerable<Address>>>();

        Assert.NotNull(customerAddresses);
        Assert.NotNull(customerAddresses.Data);
        Assert.Single(customerAddresses.Data);

        var existingAddress = customerAddresses.Data.First();
        var updateRequest = new AddressEditingRequest
        {
            PostalCode = "87654321",
            Number = "99",
            Complement = "Updated Complement"
        };

        // act: update the address
        var updateResponse = await scopedClient.PutAsJsonAsync($"api/profile/addresses/{existingAddress.Id}", updateRequest);

        // assert: ensure that the response is successful
        updateResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // assert: ensure that the address was updated
        var updatedAddress = await dbContext.Addresses
            .Where(address => address.Id == existingAddress.Id)
            .FirstOrDefaultAsync();

        Assert.NotNull(updatedAddress);

        Assert.Equal("99", updatedAddress.Number);
        Assert.Equal("Updated Complement", updatedAddress.Complement);
    }

    public async Task DisposeAsync() => await Task.CompletedTask;
    public async Task InitializeAsync()
    {
        _factory.CleanUp();
        await Task.CompletedTask;
    }
}