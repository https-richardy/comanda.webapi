namespace Comanda.TestingSuite.EndToEnd;

public sealed class CategoryEndpointTests : WebApiFixture
{
    private string _bearerToken = string.Empty;
    private HttpClient _authenticatedClient = null!;

    public CategoryEndpointTests(WebApiFactoryFixture<Program> factory) : base(factory)
    {
        AuthenticateAdminUserAsync().GetAwaiter().GetResult();
    }

    [Fact(DisplayName = "Given a valid request, it must then create a new category")]
    public async Task GivenAValidRequestItMustThenCreateANewCategory()
    {
        var client = GetAuthenticatedClient();
        var payload = new CategoryCreationRequest
        {
            Title = "Food"
        };

        var response = await client.PostAsJsonAsync("api/categories", payload);
        var responseContent = await response.Content.ReadFromJsonAsync<Response>();

        response.EnsureSuccessStatusCode();

        Assert.NotNull(responseContent);
        Assert.True(responseContent.IsSuccess);
    }

    private async Task AuthenticateAdminUserAsync()
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

    private HttpClient GetAuthenticatedClient()
    {
        if (_authenticatedClient is null)
            throw new Exception("Authenticated client not initialized.");

        return _authenticatedClient;
    }
}