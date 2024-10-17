namespace Comanda.TestingSuite.UnitTests.Repositories;

public sealed class OrderRepositoryTests : SqliteDatabaseFixture<ComandaDbContext>
{
    private readonly IOrderRepository _repository;

    public OrderRepositoryTests()
    {
        _repository = new OrderRepository(dbContext: DbContext);
    }

    [Fact(DisplayName = "Given new order, should save successfully in the database")]
    public async Task GivenNewOrder_ShouldSaveSuccessfullyInTheDatabase()
    {
        var address = Fixture.Create<Address>();
        var customer = Fixture.Build<Customer>()
            .Without(customer => customer.Orders)
            .Create();

        await DbContext.Addresses.AddAsync(address);
        await DbContext.Customers.AddAsync(customer);

        await DbContext.SaveChangesAsync();

        var order = Fixture.Build<Order>()
            .With(order => order.ShippingAddress, address)
            .With(order => order.Customer, customer)
            .Create();

        await _repository.SaveAsync(order);
        var savedOrder = await DbContext.Orders.FirstOrDefaultAsync(order => order.Id == order.Id);

        Assert.NotNull(savedOrder);
        Assert.Equal(order.Id, savedOrder.Id);
        Assert.Equal(order.Customer, savedOrder.Customer);
        Assert.Equal(order.ShippingAddress, savedOrder.ShippingAddress);
        Assert.Equal(order.Status, savedOrder.Status);
        Assert.Equal(order.Date, savedOrder.Date);
        Assert.Equal(order.CancelledReason, savedOrder.CancelledReason);
        Assert.Equal(order.Total, savedOrder.Total);
    }

    [Fact(DisplayName = "Given valid order, should update successfully in the database")]
    public async Task GivenValidOrder_ShouldUpdateSuccessfullyInTheDatabase()
    {
        var order = Fixture.Create<Order>();

        await DbContext.Orders.AddAsync(order);
        await DbContext.SaveChangesAsync();

        order.Status = EOrderStatus.CancelledByCustomer;
        order.CancelledReason = "the snack took too long.";

        await _repository.UpdateAsync(order);
        var updatedOrder = await DbContext.Orders
            .Include(order => order.Customer)
            .Include(order => order.ShippingAddress)
            .Include(order => order.Items)
            .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(order => order.Id == order.Id);

        Assert.NotNull(updatedOrder);
        Assert.Equal(order.Status, updatedOrder.Status);
        Assert.Equal(order.CancelledReason, updatedOrder.CancelledReason);
    }

    [Fact(DisplayName = "Given order ID, should retrieve correct order")]
    public async Task GivenOrderId_ShouldRetrieveCorrectOrder()
    {
        var order = Fixture.Create<Order>();

        await DbContext.Orders.AddAsync(order);
        await DbContext.SaveChangesAsync();

        var retrievedOrder = await _repository.RetrieveByIdAsync(order.Id);

        Assert.NotNull(retrievedOrder);
        Assert.Equal(order.Id, retrievedOrder.Id);
        Assert.Equal(order.Customer.Id, retrievedOrder.Customer.Id);
        Assert.Equal(order.Customer.FullName, retrievedOrder.Customer.FullName);
        Assert.Equal(order.ShippingAddress.PostalCode, retrievedOrder.ShippingAddress.PostalCode);
        Assert.Equal(order.Status, retrievedOrder.Status);
        Assert.Equal(order.Date, retrievedOrder.Date);
        Assert.Equal(order.CancelledReason, retrievedOrder.CancelledReason);
        Assert.Equal(order.Total, retrievedOrder.Total);
    }

    [Fact(DisplayName = "Given valid order, should delete successfully from the database")]
    public async Task GivenValidOrder_ShouldDeleteSuccessfullyFromDatabase()
    {
        var order = Fixture.Create<Order>();

        await DbContext.Orders.AddAsync(order);
        await DbContext.SaveChangesAsync();

        await _repository.DeleteAsync(order);
        var deletedOrder = await DbContext.Orders.FindAsync(order.Id);

        Assert.Null(deletedOrder);
    }

    [Fact(DisplayName = "Should retrieve paged collection of orders")]
    public async Task ShouldRetrievePagedOrders()
    {
        var orders = Fixture.CreateMany<Order>(10).ToList();

        await DbContext.Orders.AddRangeAsync(orders);
        await DbContext.SaveChangesAsync();

        var pagedOrders = await _repository.PagedAsync(1, 5);

        Assert.Equal(5, pagedOrders.Count());
    }

    [Fact(DisplayName = "Given specific criteria, should retrieve paged collection of orders matching the criteria")]
    public async Task GivenSpecificCriteria_ShouldRetrievePagedOrders()
    {
        var orders = Fixture.CreateMany<Order>(10).ToList();

        await DbContext.Orders.AddRangeAsync(orders);
        await DbContext.SaveChangesAsync();

        var pagedOrders = await _repository.PagedAsync(order => order.Status == EOrderStatus.Pending, 1, 5);

        Assert.All(pagedOrders, order => Assert.Equal(EOrderStatus.Pending, order.Status));
    }

    [Fact(DisplayName = "Given a request to count all orders, should return correct count")]
    public async Task GivenRequestToCountAllOrders_ShouldReturnCorrectCount()
    {
        var orders = Fixture.CreateMany<Order>(5).ToList();

        await DbContext.Orders.AddRangeAsync(orders);
        await DbContext.SaveChangesAsync();

        var count = await _repository.CountAsync();

        Assert.Equal(orders.Count, count);
    }

    [Fact(DisplayName = "Given specific criteria, should return correct count of matching orders")]
    public async Task GivenSpecificCriteria_ShouldReturnCorrectCountOfMatchingOrders()
    {
        var orders = Fixture.CreateMany<Order>(5).ToList();

        await DbContext.Orders.AddRangeAsync(orders);
        await DbContext.SaveChangesAsync();

        var count = await _repository.CountAsync(order => order.Status == EOrderStatus.Pending);

        Assert.Equal(orders.Count(order => order.Status == EOrderStatus.Pending), count);
    }

    [Fact(DisplayName = "Given valid order, should include related entities when retrieving")]
    public async Task GivenValidOrder_ShouldIncludeRelatedEntitiesWhenRetrieving()
    {
        var order = Fixture.Create<Order>();

        await DbContext.Orders.AddAsync(order);
        await DbContext.SaveChangesAsync();

        var retrievedOrder = await _repository.RetrieveByIdAsync(order.Id);

        Assert.NotNull(retrievedOrder);
        Assert.NotNull(retrievedOrder.Customer);
        Assert.NotNull(retrievedOrder.ShippingAddress);
        Assert.NotEmpty(retrievedOrder.Items);

        Assert.All(retrievedOrder.Items, item => 
        {
            Assert.NotNull(item.Product);
            Assert.NotEmpty(item.Additionals);
            Assert.NotEmpty(item.UnselectedIngredients);
        });
    }

    [Fact(DisplayName = "PagedAsync should skip and take orders based on provided page number and size")]
    public async Task PagedAsync_ShouldSkipAndTakeOrdersBasedOnProvidedPageNumberAndSize()
    {
        var orders = Fixture.CreateMany<Order>(20).ToList();

        await DbContext.Orders.AddRangeAsync(orders);
        await DbContext.SaveChangesAsync();

        var pageNumber = 2;
        var pageSize = 5;
        var pagedOrders = await _repository.PagedAsync(pageNumber, pageSize);

        Assert.Equal(pageSize, pagedOrders.Count());
        Assert.Equal(orders.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(o => o.Id), pagedOrders.Select(o => o.Id));
    }

    [Fact(DisplayName = "PagedAsync with predicate should skip and take orders based on provided page number, size, and criteria")]
    public async Task PagedAsync_WithPredicate_ShouldSkipAndTakeOrdersBasedOnProvidedPageNumberSizeAndCriteria()
    {
        var orders = Fixture.CreateMany<Order>(20).ToList();

        for (int index = 0; index < 5; index++)
        {
            orders[index].Status = EOrderStatus.Pending;
        }

        await DbContext.Orders.AddRangeAsync(orders);
        await DbContext.SaveChangesAsync();

        Expression<Func<Order, bool>> predicate = order => order.Status == EOrderStatus.Pending;

        const int pageNumber = 1;
        const int pageSize = 5;

        var pagedOrders = await _repository.PagedAsync(predicate, pageNumber, pageSize);

        Assert.Equal(pageSize, pagedOrders.Count());
        Assert.All(pagedOrders, order => Assert.Equal(EOrderStatus.Pending, order.Status));

        foreach (var order in pagedOrders)
        {
            var expectedOrder = orders.FirstOrDefault(o => o.Id == order.Id);

            Assert.NotNull(expectedOrder);
            Assert.Equal(EOrderStatus.Pending, order.Status);
            Assert.Equal(expectedOrder.Customer.FullName, order.Customer.FullName);
            Assert.Equal(expectedOrder.ShippingAddress.PostalCode, order.ShippingAddress.PostalCode);
            Assert.Equal(expectedOrder.Total, order.Total);
            Assert.Equal(expectedOrder.Date, order.Date);
            Assert.Equal(expectedOrder.CancelledReason, order.CancelledReason);

            for (int index = 0; index < order.Items.Count; index++)
            {
                var item = order.Items.ToList()[index];
                var expectedItem = expectedOrder.Items.FirstOrDefault(i => i.Id == item.Id);

                Assert.NotNull(expectedItem);

                Assert.Equal(expectedItem.Product.Id, item.Product.Id);
                Assert.Equal(expectedItem.Product.Title, item.Product.Title);
                Assert.Equal(expectedItem.Product.Description, item.Product.Description);
                Assert.Equal(expectedItem.Product.Price, item.Product.Price);

                Assert.Equal(expectedItem.Quantity, item.Quantity);
                Assert.Equal(expectedItem.Additionals.Count, item.Additionals.Count);
            }
        }
    }
}