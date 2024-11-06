namespace Comanda.TestingSuite.EndToEnd;

public sealed class ProductEndpointTests :
    IClassFixture<ApiIntegrationBase<Program, ComandaDbContext>>,
    IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly IFixture _fixture;
    private readonly ApiIntegrationBase<Program, ComandaDbContext> _factory;

    public ProductEndpointTests(ApiIntegrationBase<Program, ComandaDbContext> factory)
    {
        _factory = factory;

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _httpClient = _factory.CreateClient();
    }

    [Fact(DisplayName = "Given a valid request, it must then create a new Product")]
    public async Task GivenAValidRequestItMustThenCreateANewProduct()
    {
        // arrange: obtaining the necessary services
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: creating a category first

        var category = _fixture.Create<Category>();

        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        // arrange: authenticate httpClient as administrator
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: send a request to create a new product
        var payload = _fixture.Build<ProductCreationRequest>()
            .With(payload => payload.CategoryId, category.Id)
            .Without(payload => payload.Ingredients)
            .Create();

        var response = await authenticatedClient.PostAsJsonAsync("api/products", payload);
        var responseContent = await response.Content.ReadFromJsonAsync<Response<ProductCreationResponse>>();

        response.EnsureSuccessStatusCode();

        // assert: check if the product was created

        Assert.NotNull(responseContent);
        Assert.NotNull(responseContent.Data);

        Assert.True(responseContent.IsSuccess);
        Assert.True(responseContent.Data.ProductId > 0);

        var createdProduct = await dbContext.Products.FirstAsync();

        Assert.Equal(payload.Title, createdProduct.Title);
        Assert.Equal(payload.Description, createdProduct.Description);
        Assert.Equal(payload.CategoryId, createdProduct.Category.Id);
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