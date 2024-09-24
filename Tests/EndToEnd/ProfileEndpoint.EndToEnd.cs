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