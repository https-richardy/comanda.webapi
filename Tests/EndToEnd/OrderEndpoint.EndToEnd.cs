namespace Comanda.TestingSuite.EndToEnd;

public sealed class OrderEndpointTests :
    IClassFixture<ApiIntegrationBase<Program, ComandaDbContext>>,
    IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly ApiIntegrationBase<Program, ComandaDbContext> _factory;
    private readonly IFixture _fixture;

    public OrderEndpointTests(ApiIntegrationBase<Program, ComandaDbContext> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact(DisplayName = "Given existing orders, when fetching orders, then it must return a list of orders")]
    public async Task GivenExistingOrdersWhenFetchingOrdersThenItMustReturnListOfOrders()
    {
        // arrange: obtaining the necessary services
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: creating some orders
        var orders = _fixture.Build<Order>()
            .With(order => order.Status, EOrderStatus.Pending)
            .CreateMany(10)
            .ToList();

        await dbContext.Orders.AddRangeAsync(orders);
        await dbContext.SaveChangesAsync();

        var retrievedOrders = await dbContext.Orders.ToListAsync();

        Assert.NotNull(retrievedOrders);
        Assert.NotEmpty(retrievedOrders);

        // arrange: authenticate httpClient as administrator
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: send a request to obtain the orders with query parameters
        var queryParams = new Dictionary<string, string>
        {
            { "status", "0" },
            { "pageNumber", "1" },
            { "pageSize", "10" }
        };

        var urlEncodedContent = new FormUrlEncodedContent(queryParams);
        var queryString = await urlEncodedContent.ReadAsStringAsync();

        var response = await authenticatedClient.GetAsync($"api/orders?{queryString}");
        var content = await response.Content.ReadFromJsonAsync<Response<PaginationHelper<FormattedOrder>>>();

        response.EnsureSuccessStatusCode();

        Assert.NotNull(response);
        Assert.NotNull(content);
        Assert.NotNull(content.Data);

        Assert.NotNull(content.Data.Results);
        Assert.NotEmpty(content.Data.Results);
        Assert.Equal(10, content.Data.Results.Count());
    }

    [Fact(DisplayName = "Should not allow anonymous and authenticated customers to retrieve orders")]
    public async Task ShouldNotAllowAnonymousAndAuthenticatedCustomersToRetrieveOrders()
    {
        // arrange: authenticate httpClient as customer
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "john.doe@email.com",
            Password = "JohnDoe1234*"
        });

        // act and assert: send a request to obtain the orders with query parameters
        var queryParams = new Dictionary<string, string>
        {
            { "status", "0" },
            { "pageNumber", "1" },
            { "pageSize", "10" }
        };

        var urlEncodedContent = new FormUrlEncodedContent(queryParams);
        var queryString = await urlEncodedContent.ReadAsStringAsync();

        var response = await authenticatedClient.GetAsync($"api/orders?{queryString}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        // arrange: set httpClient to be anonymous
        var anonymousClient = _factory.CreateClient();

        // act and assert: send a request to obtain the orders with query parameters

        response = await anonymousClient.GetAsync($"api/orders?{queryString}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    public async Task DisposeAsync() => await Task.CompletedTask;
    public async Task InitializeAsync()
    {
        _factory.CleanUp();

        # region creating an administrator for each test

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

        #endregion

        #region creating an customer for each test

        var signupCredentials = new AccountRegistrationRequest
        {
            Email = "john.doe@email.com",
            Name = "John Doe",
            Password = "JohnDoe1234*"
        };

        var response = await _httpClient.PostAsJsonAsync("api/identity/register", signupCredentials);
        response.EnsureSuccessStatusCode();

        #endregion
    }
}