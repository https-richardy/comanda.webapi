namespace Comanda.TestingSuite.UnitTests.Repositories;

public sealed class CartRepositoryTests : SqliteDatabaseFixture<ComandaDbContext>
{
    private readonly ICartRepository _repository;

    public CartRepositoryTests()
    {
        _repository = new CartRepository(dbContext: DbContext);
    }

    [Fact(DisplayName = "Given a new cart, should save successfully in the database")]
    public async Task GivenNewCart_ShouldSaveSuccessfullyInTheDatabase()
    {
        var cart = Fixture.Create<Cart>();

        await _repository.SaveAsync(cart);
        var savedCart = await DbContext.Carts.FindAsync(cart.Id);

        Assert.NotNull(savedCart);
        Assert.Equal(cart.Id, savedCart.Id);
        Assert.Equal(cart.Customer.Id, savedCart.Customer.Id);
    }

    [Fact(DisplayName = "Given a cart with items, should add item successfully")]
    public async Task GivenCartWithItems_ShouldAddItemSuccessfully()
    {
        var startItems = Fixture.Build<CartItem>()
            .Without(item => item.Additionals)
            .CreateMany()
            .ToList();

        var cart = Fixture.Build<Cart>()
            .With(cart => cart.Items, startItems)
            .Create();

        var item = Fixture.Create<CartItem>();

        await DbContext.Carts.AddAsync(cart);
        await DbContext.SaveChangesAsync();

        await _repository.AddItemAsync(cart, item);

        var updatedCart = await DbContext.Carts
            .Include(cart => cart.Items)
            .FirstOrDefaultAsync(cart => cart.Id == cart.Id);

        Assert.NotNull(updatedCart);
        Assert.Contains(item, updatedCart.Items);
    }

    [Fact(DisplayName = "Given a cart with items, should remove item successfully")]
    public async Task GivenCartWithItems_ShouldRemoveItemSuccessfully()
    {
        var cart = Fixture.Create<Cart>();
        var item = Fixture.Create<CartItem>();

        cart.Items.Add(item);

        await DbContext.Carts.AddAsync(cart);
        await DbContext.SaveChangesAsync();

        await _repository.RemoveItemAsync(cart, item);
        var updatedCart = await DbContext.Carts
            .Include(cart => cart.Items)
            .FirstAsync(cart => cart.Id == cart.Id);

        Assert.DoesNotContain(item, updatedCart.Items);
    }

    [Fact(DisplayName = "Given a cart, should clear all items successfully")]
    public async Task GivenCart_ShouldClearAllItemsSuccessfully()
    {
        var items = Fixture.CreateMany<CartItem>(3).ToList();
        var cart = Fixture.Build<Cart>()
            .With(cart => cart.Items, items)
            .Create();

        DbContext.Carts.Add(cart);
        await DbContext.SaveChangesAsync();

        await _repository.ClearCartAsync(cart);
        var updatedCart = await DbContext.Carts
            .Include(cart => cart.Items)
            .FirstAsync(cart => cart.Id == cart.Id);

        Assert.Empty(updatedCart.Items);
    }

    [Fact(DisplayName = "Given a valid customer ID, should find cart with items successfully")]
    public async Task GivenValidCustomerId_ShouldFindCartWithItemsSuccessfully()
    {
        var items = Fixture.CreateMany<CartItem>(3).ToList();
        var cart = Fixture.Build<Cart>()
            .With(cart => cart.Items, items)
            .Create();

        await DbContext.Carts.AddAsync(cart);
        await DbContext.SaveChangesAsync();

        var foundCart = await _repository.FindCartWithItemsAsync(cart.Customer.Id);

        Assert.NotNull(foundCart);
        Assert.Equal(cart.Id, foundCart.Id);
        Assert.Equal(cart.Items.Count, foundCart.Items.Count);

        var foundedCartItems = foundCart.Items.ToList();
        for (int index = 0; index < cart.Items.Count; index++)
        {
            var expectedItem = items[index];
            var actualItem = foundedCartItems[index];

            Assert.Equal(expectedItem.Id, actualItem.Id);
            Assert.Equal(expectedItem.Product.Id, actualItem.Product.Id);
            Assert.Equal(expectedItem.Quantity, actualItem.Quantity);
        }
    }

    [Fact(DisplayName = "Given a valid customer ID, should find cart without items successfully")]
    public async Task GivenValidCustomerId_ShouldFindCartWithoutItemsSuccessfully()
    {
        var cart = Fixture.Create<Cart>();

        await DbContext.Carts.AddAsync(cart);
        await DbContext.SaveChangesAsync();

        var foundCart = await _repository.FindCartByCustomerIdAsync(cart.Customer.Id);

        Assert.NotNull(foundCart);
        Assert.Equal(cart.Id, foundCart.Id);
        Assert.Empty(foundCart.Items);
    }

    [Fact(DisplayName = "Given a valid cart ID, should retrieve cart by ID successfully")]
    public async Task GivenValidCartId_ShouldRetrieveCartByIdSuccessfully()
    {
        var items = Fixture.CreateMany<CartItem>(3).ToList();
        var cart = Fixture.Build<Cart>()
            .With(cart => cart.Items, items)
            .Create();

        await DbContext.Carts.AddAsync(cart);
        await DbContext.SaveChangesAsync();

        var foundCart = await _repository.RetrieveByIdAsync(cart.Id);

        Assert.NotNull(foundCart);
        Assert.Equal(cart.Id, foundCart.Id);
        Assert.Equal(cart.Customer.Id, foundCart.Customer.Id);
        Assert.Equal(cart.Items.Count, foundCart.Items.Count);
    }
}