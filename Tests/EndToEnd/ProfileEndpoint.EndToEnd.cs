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

    [Fact(DisplayName = "Should return 404 if trying to update a non-existent address")]
    public async Task ShouldReturn404IfAddressDoesNotExist()
    {
        // arrange: creating the customer first
        var signupCredentials = _fixture.Build<AccountRegistrationRequest>()
            .With(credential => credential.Name, "John Doe")
            .With(credential => credential.Email, "john@doe.com")
            .With(credential => credential.Password, "JohnDoe1234*")
            .Create();

        // arrange: registering the customer
        var signupResult = await _httpClient.PostAsJsonAsync("api/identity/register", signupCredentials);
        signupResult.EnsureSuccessStatusCode();

        // arrange: authenticate httpClient as customer
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "john@doe.com",
            Password = "JohnDoe1234*"
        });

        const int nonExistentAddressId = 999;
        var updateRequest = new AddressEditingRequest
        {
            PostalCode = "00000000",
            Number = "0",
            Complement = "Non-existent Complement"
        };

        // act: try to update an address that does not exist
        var response = await authenticatedClient.PutAsJsonAsync($"api/profile/addresses/{nonExistentAddressId}", updateRequest);

        // assert: verify that the response status code is 404
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Should delete an existing address and verify it is removed from the customer")]
    public async Task ShouldDeleteExistingAddressAndVerifyItIsRemoved()
    {
        // arrange: create authenticated customer and a new address
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        var signupCredentials = _fixture.Build<AccountRegistrationRequest>()
            .With(credential => credential.Name, "Alex Doe")
            .With(credential => credential.Email, "alex@doe.com")
            .With(credential => credential.Password, "AlexDoe123*")
            .Create();

        var signupResult = await _httpClient.PostAsJsonAsync("api/identity/register", signupCredentials);
        signupResult.EnsureSuccessStatusCode();

        var addresses = _fixture
            .CreateMany<Address>(1)
            .ToList();

        await dbContext.Addresses.AddRangeAsync(addresses);
        await dbContext.SaveChangesAsync();

        var customer = await dbContext.Customers
            .Where(customer => customer.Account.Email == "alex@doe.com")
            .FirstOrDefaultAsync();

        Assert.NotNull(customer);

        customer.Addresses = addresses;

        dbContext.Customers.Update(customer);
        await dbContext.SaveChangesAsync();

        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "alex@doe.com",
            Password = "AlexDoe123*"
        });

        var addressToDelete = customer.Addresses.First();

        // act: send request to delete the address
        var deleteResponse = await authenticatedClient.DeleteAsync($"api/profile/addresses/{addressToDelete.Id}");

        // assert: ensure the address was deleted successfully
        deleteResponse.EnsureSuccessStatusCode();

        // act: send request to get the customer's addresses after deletion
        var customerResponse = await authenticatedClient.GetAsync("api/profile/addresses");
        customerResponse.EnsureSuccessStatusCode();

        // assert: check that the address list no longer contains the deleted address
        var customerAddresses = await customerResponse.Content.ReadFromJsonAsync<Response<IEnumerable<Address>>>();

        Assert.NotNull(customerAddresses);
        Assert.NotNull(customerAddresses.Data);
        Assert.Empty(customerAddresses.Data); // No addresses should remain
    }

    [Fact(DisplayName = "Should return 404 if trying to delete a non-existent address")]
    public async Task ShouldReturn404IfDeletingNonExistentAddress()
    {
        // arrange: Create and authenticate a customer
        var signupCredentials = _fixture.Build<AccountRegistrationRequest>()
            .With(credential => credential.Name, "Jamie Doe")
            .With(credential => credential.Email, "jamie@doe.com")
            .With(credential => credential.Password, "JamieDoe123*")
            .Create();

        var signupResult = await _httpClient.PostAsJsonAsync("api/identity/register", signupCredentials);
        signupResult.EnsureSuccessStatusCode();

        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "jamie@doe.com",
            Password = "JamieDoe123*"
        });

        const int nonExistentAddressId = 999;

        // act: Send request to delete a non-existent address
        var deleteResponse = await authenticatedClient.DeleteAsync($"api/profile/addresses/{nonExistentAddressId}");

        // assert: Ensure the response is 404 Not Found
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    [Fact(DisplayName = "Should return current orders of an authenticated customer")]
    public async Task ShouldReturnCurrentOrdersOfAuthenticatedCustomer()
    {
        // arrange: create and authenticate a customer, add some orders
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

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

        // create orders
        var customer = await dbContext.Customers
            .Where(customer => customer.Account.Email == "john@doe.com")
            .FirstOrDefaultAsync();

        Assert.NotNull(customer);

        var orders = _fixture.Build<Order>()
            .With(order => order.Customer, customer)
            .With(order => order.CustomerName, customer.FullName)
            .With(order => order.Status, EOrderStatus.Pending)
            .CreateMany(2)
            .ToList();

        await dbContext.Orders.AddRangeAsync(orders);
        await dbContext.SaveChangesAsync();

        // act: Send request to get the current orders
        var response = await authenticatedClient.GetAsync("api/profile/orders");
        var responseContent = await response.Content.ReadFromJsonAsync<Response<IEnumerable<FormattedOrder>>>();

        response.EnsureSuccessStatusCode();

        Assert.NotNull(responseContent);
        Assert.NotNull(responseContent.Data);
        Assert.Equal(2, responseContent.Data.Count());

        Assert.All(responseContent.Data, order =>
        {
            Assert.Equal(customer.FullName, order.Customer);
            Assert.Equal(EOrderStatus.Pending, order.Status);

            Assert.True(order.Total > 0);
            Assert.False(string.IsNullOrEmpty(order.ShippingAddress));
        });
    }

    [Fact(DisplayName = "Should return an empty list when the customer has no current orders")]
    public async Task ShouldReturnEmptyListWhenCustomerHasNoCurrentOrders()
    {
        // arrange: create and authenticate a customer
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        var signupCredentials = _fixture.Build<AccountRegistrationRequest>()
            .With(credential => credential.Name, "Jane Doe")
            .With(credential => credential.Email, "jane@doe.com")
            .With(credential => credential.Password, "JaneDoe123*")
            .Create();

        var signupResult = await _httpClient.PostAsJsonAsync("api/identity/register", signupCredentials);
        signupResult.EnsureSuccessStatusCode();

        // arrange: authenticate httpClient as customer
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "jane@doe.com",
            Password = "JaneDoe123*"
        });

        var customer = await dbContext.Customers
            .Where(customer => customer.Account.Email == "jane@doe.com")
            .FirstOrDefaultAsync();

        // act: Send request to get the current orders
        var response = await authenticatedClient.GetAsync("api/profile/orders");
        var responseContent = await response.Content.ReadFromJsonAsync<Response<IEnumerable<FormattedOrder>>>();

        // assert: Ensure the response is empty
        response.EnsureSuccessStatusCode();

        Assert.NotNull(responseContent);
        Assert.NotNull(responseContent.Data);
        Assert.Empty(responseContent.Data);
    }

    [Fact(DisplayName = "Should return 401 Unauthorized if customer is not authenticated")]
    public async Task ShouldReturnUnauthorizedIfCustomerIsNotAuthenticated()
    {
        // act: Send request to get the current orders without authentication
        var response = await _httpClient.GetAsync("api/profile/orders");

        // assert: Ensure the response is Unauthorized
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    public async Task DisposeAsync() => await Task.CompletedTask;
    public async Task InitializeAsync()
    {
        _factory.CleanUp();
        await Task.CompletedTask;
    }
}