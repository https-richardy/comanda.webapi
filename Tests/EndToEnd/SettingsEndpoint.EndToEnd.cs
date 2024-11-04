namespace Comanda.TestingSuite.EndToEnd;

public sealed class SettingsEndpointTests :
    IClassFixture<ApiIntegrationBase<Program, ComandaDbContext>>,
    IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly ApiIntegrationBase<Program, ComandaDbContext> _factory;

    public SettingsEndpointTests(ApiIntegrationBase<Program, ComandaDbContext> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Fact(DisplayName = "Should return the current settings")]
    public async Task ShouldReturnTheCurrentSettings()
    {
        // arrange: authenticate httpClient as administrator
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: send a request to obtain the system settings.

        var response = await authenticatedClient.GetAsync("api/settings");
        var content = await response.Content.ReadFromJsonAsync<Response<SettingsFormattedResponse>>();

        response.EnsureSuccessStatusCode();

        Assert.NotNull(content);
        Assert.NotNull(content.Data);

        Assert.True(content.IsSuccess);
        Assert.False(content.Data.AcceptAutomatically);

        // check in the entity settings (with fluent API) that they already have default values.

        Assert.Equal(5, content.Data.MaxConcurrentAutomaticOrders);
        Assert.Equal(30, content.Data.EstimatedDeliveryTimeInMinutes);
        Assert.Equal(0.0m, content.Data.DeliveryFee);
    }

    [Fact(DisplayName = "Should update settings with a valid request")]
    public async Task ShouldUpdateSettingsWithValidRequest()
    {
        // arrange: authenticate httpClient as administrator
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        var updatePayload = new SettingsEditingRequest
        {
            AcceptAutomatically = true,
            MaxConcurrentAutomaticOrders = 10,
            EstimatedDeliveryTimeInMinutes = 45,
            DeliveryFee = 9.99m
        };

        // act: send PUT request to update endpoint

        var response = await authenticatedClient.PutAsJsonAsync("api/settings", updatePayload);
        var content = await response.Content.ReadFromJsonAsync<Response>();

        response.EnsureSuccessStatusCode();

        Assert.NotNull(content);
        Assert.True(content.IsSuccess);

        // act: send a request to obtain the current settings and make sure it has been updated.

        response = await authenticatedClient.GetAsync("api/settings");
        var responseContent = await response.Content.ReadFromJsonAsync<Response<SettingsFormattedResponse>>();

        Assert.NotNull(responseContent);
        Assert.NotNull(responseContent.Data);

        Assert.True(responseContent.IsSuccess);
        Assert.True(responseContent.Data.AcceptAutomatically);

        Assert.Equal(10, responseContent.Data.MaxConcurrentAutomaticOrders);
        Assert.Equal(45, responseContent.Data.EstimatedDeliveryTimeInMinutes);
        Assert.Equal(9.99m, responseContent.Data.DeliveryFee);
    }

    [Fact(DisplayName = "Should not allow unauthorized access")]
    public async Task ShouldNotAllowUnauthorizedAccess()
    {
        var response = await _httpClient.GetAsync("api/settings");

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        response = await _httpClient.PutAsync("api/settings", null);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    public async Task DisposeAsync() => await Task.CompletedTask;
    public async Task InitializeAsync()
    {
        _factory.CleanUp();

        var services = _factory.GetServiceProvider();

        var userManager = services.GetService<UserManager<Account>>();
        var roleManager = services.GetService<RoleManager<IdentityRole>>();

        Assert.NotNull(userManager);
        Assert.NotNull(roleManager);

        var admin = new Account
        {
            UserName = "Comanda Administrator",
            Email = "comanda@admin.com",
        };

        var result = await userManager.CreateAsync(admin, "ComandaAdministrator123*");
        if (result.Succeeded is true)
        {
            const string adminRoleName = "Administrator";
            var adminRoleExists = await roleManager.RoleExistsAsync(adminRoleName);
            Assert.False(adminRoleExists);

            if (adminRoleExists is false)
            {
                var adminRole = new IdentityRole(adminRoleName);
                await roleManager.CreateAsync(adminRole);
            }

            await userManager.AddToRoleAsync(admin, adminRoleName);
        }
    }
}