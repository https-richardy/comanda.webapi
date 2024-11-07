namespace Comanda.TestingSuite.EndToEnd;

public sealed class AdditionalsEndpointEndToEndTestSuite :
    IClassFixture<ApiIntegrationBase<Program, ComandaDbContext>>,
    IAsyncLifetime
{
    private readonly ApiIntegrationBase<Program, ComandaDbContext> _factory;
    private readonly IFixture _fixture;
    private readonly HttpClient _httpClient;

    public AdditionalsEndpointEndToEndTestSuite(ApiIntegrationBase<Program, ComandaDbContext> factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact(DisplayName = "Given a request to the additionals listing endpoint, it should return a successful response")]
    public async Task GivenARequestToTheAdditionalsEndpoint_WhenTheRequestIsSent_ThenASuccessfulResponseShouldBeReturned()
    {
        // arrange: obtaining the necessary services

        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: creating some additionals
        const int additionalQuantity = 5;

        var category = _fixture.Create<Category>();

        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        var additionals = _fixture.Build<Additional>()
            .With(additional => additional.Category, category)
            .CreateMany(additionalQuantity);

        await dbContext.Additionals.AddRangeAsync(additionals);
        await dbContext.SaveChangesAsync();

        // arrange: authenticate httpClient as administrator
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: fetch additionals
        var response = await authenticatedClient.GetAsync("api/additionals");
        var content = await response.Content.ReadFromJsonAsync<Response<IEnumerable<Additional>>>();

        response.EnsureSuccessStatusCode();

        Assert.NotNull(content);
        Assert.NotNull(content.Data);

        Assert.Equal(additionalQuantity, content.Data.Count());

        Assert.All(content.Data, additional =>
        {
            Assert.Equal(additional.Category.Id, category.Id);
            Assert.True(additional.Price > 0);
            Assert.False(string.IsNullOrEmpty(additional.Name));
        });
    }

    [Fact(DisplayName = "Given a category, when requesting the additionals listing, it should return the available additionals for that category")]
    public async Task GivenACategory_WhenRequestingAdditionalsListing_ThenAvailableAdditionalsShouldBeReturned()
    {
        // arrange: obtaining the necessary services
        var services = _factory.GetServiceProvider();
        var dbContext = _factory.GetDbContext();

        // arrange: creating categories and additionals
        const int additionalQuantity = 5;

        var otherCategory = _fixture.Create<Category>();
        var snackCategory = _fixture.Build<Category>()
            .With(category => category.Name, "snack")
            .Create();

        var categories = new List<Category> { snackCategory, otherCategory };

        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();

        var snackAdditionals = _fixture.Build<Additional>()
            .With(additional => additional.Category, snackCategory)
            .CreateMany(additionalQuantity)
            .ToList();

        var otherAdditionals = _fixture.Build<Additional>()
            .With(additional => additional.Category, otherCategory)
            .CreateMany(10)
            .ToList();

        var additionals = snackAdditionals.Concat(otherAdditionals).ToList();

        await dbContext.Additionals.AddRangeAsync(additionals);
        await dbContext.SaveChangesAsync();

        // arrange: authenticate httpClient as administrator
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: fetching additionals for the snack category
        var queryParams = new Dictionary<string, string>
        {
            { "categoryId", $"{snackCategory.Id}" },
        };

        var urlEncodedContent = new FormUrlEncodedContent(queryParams);
        var queryString = await urlEncodedContent.ReadAsStringAsync();

        var response = await authenticatedClient.GetFromJsonAsync<Response<IEnumerable<Additional>>>($"api/additionals/search?{queryString}");

        // assert: verifying the response and data
        Assert.NotNull(response);
        Assert.NotNull(response.Data);
        Assert.NotEmpty(response.Data);
        Assert.Equal(additionalQuantity, response.Data.Count());
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