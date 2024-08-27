namespace Comanda.TestingSuite.UnitTests.Repositories;

public sealed class CartRepositoryTests : InMemoryDatabaseFixture<ComandaDbContext>
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
}