namespace Comanda.TestingSuite.Integration.Services;

public sealed class ProfileDataExportServiceIntegrationTest : IntegrationFixture<ComandaDbContext>
{
    private readonly IFixture _fixture;
    private readonly IProfileDataExportService _profileDataExportService;

    public ProfileDataExportServiceIntegrationTest()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _profileDataExportService = ServiceProvider.GetRequiredService<IProfileDataExportService>();
    }

    [Fact(DisplayName = "ExportProfileDataAsync should export profile data correctly")]
    public async Task ExportProfileDataAsyncShouldExportProfileDataCorrectly()
    {
        var addresses = _fixture
            .Build<Address>()
            .CreateMany(2)
            .ToList();

        var user = _fixture.Build<Account>()
            .With(user => user.Id, Guid.NewGuid().ToString())
            .With(user => user.UserName, "John Doe")
            .With(user => user.Email, "john@doe.com")
            .Create();

        var customer = _fixture.Build<Customer>()
            .With(customer => customer.Account, user)
            .With(customer => customer.Addresses, addresses)
            .Without(customer => customer.Orders)
            .Create();

        var orders = _fixture.Build<Order>()
            .With(order => order.Customer, customer)
            .CreateMany(4)
            .ToList();

        await DbContext.Addresses.AddRangeAsync(addresses);
        await DbContext.Orders.AddRangeAsync(orders);
        await DbContext.Users.AddAsync(user);
        await DbContext.Customers.AddAsync(customer);
        await DbContext.SaveChangesAsync();

        var result = await _profileDataExportService.ExportDataAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.UserName, result.Name);
        Assert.Equal(user.Email, result.Email);

        Assert.Equal(addresses.Count, result.Addresses.Count());
        Assert.Equal(orders.Count, result.Orders.Count());
    }
}