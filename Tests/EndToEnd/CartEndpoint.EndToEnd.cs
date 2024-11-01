namespace Comanda.TestingSuite.EndToEnd;

public sealed class CartEndpointTests :
    IClassFixture<ApiIntegrationBase<Program, ComandaDbContext>>,
    IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly IFixture _fixture;
    private readonly ApiIntegrationBase<Program, ComandaDbContext> _factory;

    public CartEndpointTests(ApiIntegrationBase<Program, ComandaDbContext> factory)
    {
        _factory = factory;

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _httpClient = _factory.CreateClient();
    }

    [Fact(DisplayName = "Given a product, when adding it to a cart, then it should be persisted in the database")]
    public async Task GivenProduct_WhenAddingItToCart_ThenItShouldBePersistedInDatabase()
    {
        /* arrange: obtaining the necessary services for the scenario. */

        var services = _factory.GetServiceProvider();
        var dbContext = _factory.GetDbContext();

        /* act: making sure there are no items in the cart */

        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "john.doe@email.com",
            Password = "JohnDoe1234*"
        });

        var response = await authenticatedClient.GetFromJsonAsync<Response<CartResponse>>("api/cart");

        Assert.NotNull(response);
        Assert.Null(response.Data);

        /* act: adding a product to the cart */

        var product = _fixture.Create<Product>();
        var cartItem = _fixture.Build<CartItem>()
            .With(item => item.Product, product)
            .Create();

        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();

        var request = new InsertProductIntoCartRequest { ProductId = product.Id, Quantity = 1 };
        var result = await authenticatedClient.PostAsJsonAsync("api/cart/items", request);

        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);

        /* act: getting the cart */

        response = await authenticatedClient.GetFromJsonAsync<Response<CartResponse>>("api/cart");

        Assert.NotNull(response);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data.Items);
    }

    public async Task DisposeAsync() => await Task.CompletedTask;
    public async Task InitializeAsync()
    {
        _factory.CleanUp();

        var response = await _httpClient.PostAsJsonAsync("api/identity/register", new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = "john.doe@email.com",
            Password = "JohnDoe1234*"
        });

        response.EnsureSuccessStatusCode();
    }
}