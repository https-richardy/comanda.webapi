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

    [Fact(DisplayName = "Given an existing category, when updating it, it should return 200 OK and update the category")]
    public async Task GivenAnExistingCategory_WhenUpdatingIt_ThenItShouldReturnOkAndTheCategoryShouldBeUpdated()
    {
        // arrange: create a category and add it to the database
        var category = _fixture.Create<Category>();
        var dbContext = _factory.GetDbContext();

        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        // arrange: authenticate client
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: prepare update request and send it
        var updateRequest = _fixture.Build<CategoryEditingRequest>()
            .With(payload => payload.Title, "Updated Category")
            .Create();

        var response = await authenticatedClient.PutAsJsonAsync($"api/categories/{category.Id}", updateRequest);
        response.EnsureSuccessStatusCode();

        // assert: verify that the category was updated
        var httpResponse = await authenticatedClient.GetAsync($"api/categories/{category.Id}");
        var content = await httpResponse.Content.ReadFromJsonAsync<Response<Category>>();

        Assert.NotNull(content);
        Assert.NotNull(content.Data);
        Assert.Equal(updateRequest.Title, content.Data.Name);
    }

    [Fact(DisplayName = "Given a non-existent category, when updating it, it should return 404 Not Found")]
    public async Task GivenANonExistentCategory_WhenUpdatingIt_ThenItShouldReturnNotFound()
    {
        // arrange: authenticate client
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // arrange: prepare a non-existent category id
        const int nonExistentCategoryId = 999;

        // act: prepare update request and send it
        var updateRequest = _fixture.Build<CategoryEditingRequest>()
            .With(payload => payload.Title, "Updated Category")
            .Create();

        var response = await authenticatedClient.PutAsJsonAsync($"api/categories/{nonExistentCategoryId}", updateRequest);

        // assert: verify that 404 Not Found is returned
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Given an existing category, when deleting it, it should return 200 OK")]
    public async Task GivenAnExistingCategory_WhenDeletingIt_ThenItShouldReturnOk()
    {
        // arrange: create a category and add it to the database
        var category = _fixture.Create<Category>();
        var dbContext = _factory.GetDbContext();

        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        // arrange: authenticate client
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: delete the category
        var response = await authenticatedClient.DeleteAsync($"api/categories/{category.Id}");
        response.EnsureSuccessStatusCode();

        // assert: verify that the category is deleted
        response = await authenticatedClient.GetAsync($"api/categories/{category.Id}");
        var deletedCategory = await response.Content.ReadFromJsonAsync<Response<Category>>();

        Assert.NotNull(deletedCategory);
        Assert.Null(deletedCategory.Data);
    }

    [Fact(DisplayName = "Given a non-existent category, when deleting it, it should return 404 Not Found")]
    public async Task GivenANonExistentCategory_WhenDeletingIt_ThenItShouldReturnNotFound()
    {
        // arrange: authenticate client
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // arrange: prepare a non-existent category id
        const int nonExistentCategoryId = 999;

        // act: attempt to delete non-existent category
        var response = await authenticatedClient.DeleteAsync($"api/categories/{nonExistentCategoryId}");

        // assert: verify that 404 Not Found is returned
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
