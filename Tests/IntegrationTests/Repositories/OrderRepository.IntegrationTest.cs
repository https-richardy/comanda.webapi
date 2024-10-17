namespace Comanda.TestingSuite.Integration.Repositories;

public sealed class OrderRepositoryIntegrationTest : IntegrationFixture<ComandaDbContext>
{
    [Fact(DisplayName = "Given an order, should add it successfully")]
    public async Task GivenAnOrderShouldAddItSuccessfully()
    {
        var orderRepository = ServiceProvider.GetRequiredService<IOrderRepository>();
        var customerRepository = ServiceProvider.GetRequiredService<ICustomerRepository>();
        var addressRepository = ServiceProvider.GetRequiredService<IAddressRepository>();

        var address = Fixture.Create<Address>();
        await addressRepository.SaveAsync(address);

        var orderItems = Fixture
            .CreateMany<OrderItem>(5)
            .ToList();

        var customer = Fixture.Build<Customer>()
            .With(customer => customer.Addresses, [ address ])
            .Without(customer => customer.Orders)
            .Create();

        await customerRepository.SaveAsync(customer);

        var order = Fixture.Build<Order>()
            .With(order => order.Items, orderItems)
            .With(order => order.ShippingAddress, address)
            .With(order => order.Customer, customer)
            .Create();

        await orderRepository.SaveAsync(order);
        var orderRetrieved = await orderRepository.RetrieveByIdAsync(order.Id);

        Assert.NotNull(order);
        Assert.Equal(order.Id, order.Id);
        Assert.Equal(order.Customer.Id, order.Customer.Id);
        Assert.Equal(order.ShippingAddress.Id, order.ShippingAddress.Id);

        Assert.Equal(order.Status, order.Status);
        Assert.Equal(order.Date, order.Date);
        Assert.Equal(order.CancelledReason, order.CancelledReason);
        Assert.Equal(order.Total, order.Total);

        Assert.NotNull(order.Items);
        Assert.NotEmpty(order.Items);
        Assert.Equal(orderItems.Count, order.Items.Count);

        Assert.All(order.Items, item =>
        {
            Assert.NotNull(item.Product);
            Assert.NotEmpty(item.Additionals);
            Assert.NotEmpty(item.UnselectedIngredients);
        });
    }

    [Fact(DisplayName = "Given a collection of orders, should return paged results successfully")]
    public async Task GivenACollectionOfOrdersShouldReturnPagedResultsSuccessfully()
    {
        var orderRepository = ServiceProvider.GetRequiredService<IOrderRepository>();
        var customerRepository = ServiceProvider.GetRequiredService<ICustomerRepository>();
        var addressRepository = ServiceProvider.GetRequiredService<IAddressRepository>();

        var address = Fixture.Create<Address>();
        await addressRepository.SaveAsync(address);

        var orderItems = Fixture
            .CreateMany<OrderItem>(5)
            .ToList();

        var customer = Fixture.Build<Customer>()
            .Without(customer => customer.Orders)
            .With(customer => customer.Addresses, [ address ])
            .Create();

        await customerRepository.SaveAsync(customer);

        var orders = Fixture.Build<Order>()
            .With(order => order.Items, orderItems)
            .With(order => order.Customer, customer)
            .With(order => order.ShippingAddress, address)
            .CreateMany(5)
            .ToList();

        foreach (var order in orders)
        {
            Assert.NotNull(order.Customer);
            Assert.NotEmpty(order.Items);
            Assert.Equal(order.Customer.Id, customer.Id);

            await orderRepository.SaveAsync(order);
        }

        const int pageNumber = 1;
        const int pageSize = 5;

        Expression<Func<Order, bool>> predicate = order => order.Customer.Id == customer.Id;
        var pagedOrders = await orderRepository.PagedAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            predicate: predicate
        );

        Assert.NotNull(pagedOrders);
        Assert.NotEmpty(pagedOrders);

        Assert.Equal(pageSize, pagedOrders.Count());
        Assert.All(pagedOrders, order => Assert.Equal(customer.Id, order.Customer.Id));
    }
}