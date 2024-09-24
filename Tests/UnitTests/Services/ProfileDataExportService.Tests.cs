using Microsoft.Extensions.Options;

namespace Comanda.TestingSuite.UnitTests.Services;

public sealed class ProfileDataExportServiceTests
{
    private readonly Mock<UserManager<Account>> _userManagerMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IAddressRepository> _addressRepositoryMock;
    private readonly IProfileDataExportService _profileDataExportService;
    private readonly IFixture _fixture;

    public ProfileDataExportServiceTests()
    {
        _userManagerMock = new Mock<UserManager<Account>>(
            new Mock<IUserStore<Account>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<Account>>().Object,
            new IUserValidator<Account>[0],
            new IPasswordValidator<Account>[0],
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<Account>>>().Object
        );

        _orderRepositoryMock = new Mock<IOrderRepository>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _addressRepositoryMock = new Mock<IAddressRepository>();

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _profileDataExportService = new ProfileDataExportService(
            userManager: _userManagerMock.Object,
            orderRepository: _orderRepositoryMock.Object,
            customerRepository: _customerRepositoryMock.Object,
            addressRepository: _addressRepositoryMock.Object
        );
    }

    [Fact(DisplayName = "ExportDataAsync should return correct data when user and customer exist")]
    public async Task ExportDataAsync_ShouldReturnCorrectData_WhenUserAndCustomerExist()
    {
        var userId = Guid.NewGuid().ToString();
        var user = _fixture.Build<Account>()
            .With(user => user.Id, userId)
            .With(user => user.UserName, "John Doe")
            .With(user => user.Email, "john@doe.com")
            .Create();

        var customer = _fixture.Build<Customer>()
            .With(customer => customer.Account, user)
            .Create();

        var orders = _fixture
            .CreateMany<Order>(3)
            .ToList();

        var addresses = _fixture
            .CreateMany<Address>(2)
            .ToList();

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(userId))
            .ReturnsAsync(user);

        _customerRepositoryMock
            .Setup(repository => repository.FindCustomerByUserIdAsync(userId))
            .ReturnsAsync(customer);

        _orderRepositoryMock
            .Setup(repository => repository.FindAllAsync(It.IsAny<Expression<Func<Order, bool>>>()))
            .ReturnsAsync(orders);

        _addressRepositoryMock
            .Setup(repository => repository.GetAddressesByCustomerIdAsync(customer.Id))
            .ReturnsAsync(addresses);

        var result = await _profileDataExportService.ExportDataAsync(userId);

        Assert.NotNull(result);

        Assert.Equal(user.UserName, result.Name);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(orders.Count, result.Orders.Count());
        Assert.Equal(addresses.Count, result.Addresses.Count());
    }
}