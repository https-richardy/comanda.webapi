namespace Comanda.TestingSuite.Integration.Repositories;

public sealed class OrderRepositoryIntegrationTest : IntegrationFixture<ComandaDbContext>
{
    [Fact(DisplayName = "Given an order, should add it successfully")]
    public async Task GivenAnOrderShouldAddItSuccessfully()
    {
        var orderRepository = ServiceProvider.GetRequiredService<IOrderRepository>();
        var customerRepository = ServiceProvider.GetRequiredService<ICustomerRepository>();
        var addressRepository = ServiceProvider.GetRequiredService<IAddressRepository>();
        var productRepository = ServiceProvider.GetRequiredService<IProductRepository>();

        var address = Fixture.Create<Address>();
        await addressRepository.SaveAsync(address);

        var product = Fixture.Create<Product>();
        await productRepository.SaveAsync(product);

        var createdProduct = await productRepository.RetrieveByIdAsync(product.Id);

        var orderItems = Fixture.Build<OrderItem>()
            .With(item => item.Product, createdProduct)
            .CreateMany(2)
            .ToList();

        var createdAddress = await addressRepository.RetrieveByIdAsync(address.Id);
        var customer = Fixture.Build<Customer>()
            .With(customer => customer.Addresses, [ createdAddress ])
            .Without(customer => customer.Orders)
            .Create();

        await customerRepository.SaveAsync(customer);
        var createdCustomer = await customerRepository.RetrieveByIdAsync(customer.Id);

        var order = Fixture.Build<Order>()
            .With(order => order.Items, orderItems)
            .With(order => order.ShippingAddress, createdAddress)
            .With(order => order.Customer, createdCustomer)
            .Create();

        await orderRepository.SaveAsync(order);
        var orderRetrieved = await orderRepository.RetrieveByIdAsync(order.Id);

        Assert.NotNull(order);
        Assert.Equal(order.Id, orderRetrieved.Id);
        Assert.Equal(order.Customer.Id, orderRetrieved.Customer.Id);
        Assert.Equal(order.ShippingAddress.Id, orderRetrieved.ShippingAddress.Id);

        Assert.Equal(order.Status, orderRetrieved.Status);
        Assert.Equal(order.Date, orderRetrieved.Date);
        Assert.Equal(order.CancelledReason, orderRetrieved.CancelledReason);
        Assert.True(order.Total > 0);

        Assert.NotNull(order.Items);
        Assert.NotEmpty(order.Items);
        Assert.Equal(orderItems.Count, orderRetrieved.Items.Count);

        Assert.All(orderRetrieved.Items, item =>
        {
            Assert.NotNull(item.Product);
            Assert.NotNull(item.Additionals);
            Assert.NotNull(item.UnselectedIngredients);
        });
    }
}