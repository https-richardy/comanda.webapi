namespace Comanda.TestingSuite.EndToEnd;

public sealed class CategoryEndpointTests :
    IClassFixture<ApiIntegrationBase<Program, ComandaDbContext>>,
    IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly IFixture _fixture;
    private readonly ApiIntegrationBase<Program, ComandaDbContext> _factory;

    public CategoryEndpointTests(ApiIntegrationBase<Program, ComandaDbContext> factory)
    {
        _factory = factory;

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _httpClient = _factory.CreateClient();
    }

    [Fact(DisplayName = "Given a valid request, it must then create a new category")]
    public async Task GivenAValidRequestItMustThenCreateANewCategory()
    {
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        var payload = new CategoryCreationRequest
        {
            Title = "Food"
        };

        var response = await authenticatedClient.PostAsJsonAsync("api/categories", payload);
        var responseContent = await response.Content.ReadFromJsonAsync<Response>();

        response.EnsureSuccessStatusCode();

        Assert.NotNull(responseContent);
        Assert.True(responseContent.IsSuccess);
    }

    [Fact(DisplayName = "Given existing categories, when requesting the categories endpoint, it should return the categories")]
    public async Task GivenExistingCategories_WhenRequestingTheCategoriesEndpoint_ThenItShouldReturnTheCategories()
    {
        // arrange: creating some categories
        var categories = _fixture.CreateMany<Category>(5);

        var dbContext = _factory.GetDbContext();

        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();

        // act: request the categories
        var response = await _httpClient.GetAsync("api/categories");
        var responseContent = await response.Content.ReadFromJsonAsync<Response<IEnumerable<Category>>>();

        response.EnsureSuccessStatusCode();

        Assert.NotNull(responseContent);
        Assert.NotNull(responseContent.Data);

        Assert.Equal(categories.Count(), responseContent.Data.Count());

        Assert.All(responseContent.Data, category =>
        {
            Assert.Contains(categories, existingCategory => existingCategory.Id == category.Id);
        });
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