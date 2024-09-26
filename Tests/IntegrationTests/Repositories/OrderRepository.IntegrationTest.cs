using System.Diagnostics;

namespace Comanda.TestingSuite.Integration.Repositories;

public sealed class OrderRepositoryIntegrationTest : IntegrationFixture<ComandaDbContext>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IFixture _fixture;

    public OrderRepositoryIntegrationTest()
    {
        _orderRepository = ServiceProvider.GetRequiredService<IOrderRepository>();

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact(DisplayName = "Given an order, should add it successfully")]
    public async Task GivenAnOrderShouldAddItSuccessfully()
    {
        var orderItems = _fixture
            .CreateMany<OrderItem>(5)
            .ToList();

        var order = _fixture.Build<Order>()
            .With(order => order.Items, orderItems)
            .Create();

        await _orderRepository.SaveAsync(order);
        var orderRetrieved = await _orderRepository.RetrieveByIdAsync(order.Id);

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
        var orderItems = _fixture
            .CreateMany<OrderItem>(5)
            .ToList();

        var customer = _fixture.Build<Customer>()
            .Without(customer => customer.Orders)
            .Create();

        var customerRepository = ServiceProvider.GetRequiredService<ICustomerRepository>();
        await customerRepository.SaveAsync(customer);

        var orders = _fixture.Build<Order>()
            .With(order => order.Items, orderItems)
            .With(order => order.Customer, customer)
            .CreateMany(5)
            .ToList();

        foreach (var order in orders)
        {
            Assert.NotNull(order.Customer);
            Assert.NotEmpty(order.Items);
            Assert.Equal(order.Customer.Id, customer.Id);

            await _orderRepository.SaveAsync(order);
        }

        const int pageNumber = 1;
        const int pageSize = 5;

        Expression<Func<Order, bool>> predicate = order => order.Customer.Id == customer.Id;
        var pagedOrders = await _orderRepository.PagedAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            predicate: predicate
        );

        Assert.NotNull(pagedOrders);
        Assert.All(pagedOrders, order =>
        {
            Assert.NotNull(order.Customer);
            Assert.NotNull(order.ShippingAddress);
            Assert.NotNull(order.Items);

            Assert.NotEmpty(order.Items);

            Assert.Equal(orderItems.Count, order.Items.Count);
            Assert.Equal(customer.Id, order.Customer.Id);
            Assert.Equal(customer.FullName, order.Customer.FullName);
            Assert.Contains(order.ShippingAddress, order.Customer.Addresses);

            Assert.True(order.Total > 0);
            Assert.All(order.Items, item =>
            {
                Assert.NotNull(item.Product);
                Assert.NotEmpty(item.Additionals);
                Assert.NotEmpty(item.UnselectedIngredients);


                Assert.True(!string.IsNullOrEmpty(item.Product.Title));
            });
        });
    }
}