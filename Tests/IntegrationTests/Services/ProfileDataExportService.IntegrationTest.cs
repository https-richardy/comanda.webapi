namespace Comanda.TestingSuite.Integration.Services;

public sealed class ProfileDataExportServiceIntegrationTest : IntegrationFixture<ComandaDbContext>
{
    [Fact(DisplayName = "ExportProfileDataAsync should export profile data correctly")]
    public async Task ExportProfileDataAsyncShouldExportProfileDataCorrectly()
    {
        var profileDataExportService = ServiceProvider.GetRequiredService<IProfileDataExportService>();

        var addresses = Fixture
            .Build<Address>()
            .CreateMany(2)
            .ToList();

        var user = Fixture.Build<Account>()
            .With(user => user.Id, Guid.NewGuid().ToString())
            .With(user => user.UserName, "John Doe")
            .With(user => user.Email, "john@doe.com")
            .Create();

        var customer = Fixture.Build<Customer>()
            .With(customer => customer.Account, user)
            .With(customer => customer.Addresses, addresses)
            .Without(customer => customer.Orders)
            .Create();

        var orders = Fixture.Build<Order>()
            .With(order => order.Customer, customer)
            .CreateMany(4)
            .ToList();

        await DbContext.Addresses.AddRangeAsync(addresses);
        await DbContext.Orders.AddRangeAsync(orders);
        await DbContext.Users.AddAsync(user);
        await DbContext.Customers.AddAsync(customer);
        await DbContext.SaveChangesAsync();

        var result = await profileDataExportService.ExportDataAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.UserName, result.Name);
        Assert.Equal(user.Email, result.Email);

        Assert.Equal(addresses.Count, result.Addresses.Count());
        Assert.Equal(orders.Count, result.Orders.Count());
    }
}