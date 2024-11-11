namespace Comanda.TestingSuite.EndToEnd;

[Trait("category", "E2E")]
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

    [Fact(DisplayName = "Given a product, when removing it from a cart, then it must be removed from the cart")]
    public async Task GivenProduct_WhenRemovingItFromCart_ThenItMustBeRemovedFromTheCart()
    {
        /* arrange: obtaining the necessary services for the scenario. */

        var services = _factory.GetServiceProvider();
        var dbContext = _factory.GetDbContext();

        /* act: adding an item to the cart so that there is at least one item for this scenario */

        var product = _fixture.Create<Product>();
        var cartItem = _fixture.Build<CartItem>()
            .With(item => item.Product, product)
            .Create();

        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();

        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "john.doe@email.com",
            Password = "JohnDoe1234*"
        });

        var request = new InsertProductIntoCartRequest { ProductId = product.Id, Quantity = 1 };
        var response = await authenticatedClient.PostAsJsonAsync("api/cart/items", request);

        response.EnsureSuccessStatusCode();

        /* act: checking that the item has been added. */

        var responseCart = await authenticatedClient.GetFromJsonAsync<Response<CartResponse>>("api/cart");

        Assert.NotNull(responseCart);
        Assert.NotNull(responseCart.Data);
        Assert.Single(responseCart.Data.Items);

        /* act: sending a request to delete an item from the cart */

        var deletionRequest = await authenticatedClient.DeleteAsync("api/cart/items/1");
        deletionRequest.EnsureSuccessStatusCode();

        /* act: getting the cart to check if it has indeed been deleted. */

        responseCart = await authenticatedClient.GetFromJsonAsync<Response<CartResponse>>("api/cart");

        Assert.NotNull(responseCart);
        Assert.Null(responseCart.Data);
    }

    [Fact(DisplayName = "Given an existing item in the cart, when the request to increase the quantity, then it should be updated")]
    public async Task GivenAnExistingItemInTheCart_WhenTheRequestToIncreaseTheQuantity_ThenItShouldBeUpdated()
    {
        /* arrange: obtaining the necessary services for the scenario. */

        var services = _factory.GetServiceProvider();
        var dbContext = _factory.GetDbContext();

        /* act: adding an item to the cart so that there is at least one item for this scenario */

        var product = _fixture.Create<Product>();
        var cartItem = _fixture.Build<CartItem>()
            .With(item => item.Product, product)
            .Create();

        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();

        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "john.doe@email.com",
            Password = "JohnDoe1234*"
        });

        var request = new InsertProductIntoCartRequest { ProductId = product.Id, Quantity = 1 };
        var response = await authenticatedClient.PostAsJsonAsync("api/cart/items", request);

        response.EnsureSuccessStatusCode();

        /* act: checking that the item has been added. */

        var responseCart = await authenticatedClient.GetFromJsonAsync<Response<CartResponse>>("api/cart");

        Assert.NotNull(responseCart);
        Assert.NotNull(responseCart.Data);
        Assert.Single(responseCart.Data.Items);

        /* act: sending a request to increase the quantity of the item in the cart */

        var increaseRequest = new IncrementCartItemQuantityRequest { ItemId = 1 };
        var increaseResponse = await authenticatedClient.PostAsJsonAsync("api/cart/items/1/increment", increaseRequest);

        Assert.NotNull(increaseResponse);
        Assert.True(increaseResponse.IsSuccessStatusCode);

        /* act: getting the cart to check that the quantity of the item has indeed been increased. */

        responseCart = await authenticatedClient.GetFromJsonAsync<Response<CartResponse>>("api/cart");

        Assert.NotNull(responseCart);
        Assert.NotNull(responseCart.Data);
        Assert.Single(responseCart.Data.Items);
        Assert.Equal(2, responseCart.Data.Items.First().Quantity);
    }

    [Fact(DisplayName = "Given an existing item in the cart, when the request to decrease the quantity, then it should be updated")]
    public async Task GivenAnExistingItemInTheCart_WhenTheRequestToDecreaseTheQuantity_ThenItShouldBeUpdated()
    {
        /* arrange: obtaining the necessary services for the scenario. */

        var dbContext = _factory.GetDbContext();

        /* act: add items to the cart so that there is existing data for this scenario */

        var product = _fixture.Create<Product>();

        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();

        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "john.doe@email.com",
            Password = "JohnDoe1234*"
        });

        var request = new InsertProductIntoCartRequest { ProductId = product.Id, Quantity = 2 };
        var response = await authenticatedClient.PostAsJsonAsync("api/cart/items", request);

        response.EnsureSuccessStatusCode();

        /* act: checking that the item has been added. */

        var responseCart = await authenticatedClient.GetFromJsonAsync<Response<CartResponse>>("api/cart");

        Assert.NotNull(responseCart);
        Assert.NotNull(responseCart.Data);
        Assert.Single(responseCart.Data.Items);

        /* act: sending a request to decrease the quantity of the item in the cart */

        var decreaseRequest = new DecrementCartItemQuantityRequest { ItemId = 1 };
        var decreaseResponse = await authenticatedClient.PostAsJsonAsync("api/cart/items/1/decrement", decreaseRequest);

        Assert.NotNull(decreaseResponse);
        Assert.True(decreaseResponse.IsSuccessStatusCode);

        /* act: getting the cart to check that the quantity of the item has indeed been decreased. */

        responseCart = await authenticatedClient.GetFromJsonAsync<Response<CartResponse>>("api/cart");

        Assert.NotNull(responseCart);
        Assert.NotNull(responseCart.Data);
        Assert.Single(responseCart.Data.Items);
        Assert.Equal(1, responseCart.Data.Items.First().Quantity);
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