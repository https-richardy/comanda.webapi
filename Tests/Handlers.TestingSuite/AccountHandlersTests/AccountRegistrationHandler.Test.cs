#pragma warning disable CS8625

namespace Comanda.WebApi.TestingSuite.Handlers;

public sealed class AccountRegistrationHandlerTestingSuite
{
    private readonly Mock<UserManager<Account>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly Mock<IValidator<AccountRegistrationRequest>> _validatorMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IEstablishmentOwnerRepository> _establishmentOwnerRepositoryMock;
    private readonly AccountRegistrationHandler _handler;

    public AccountRegistrationHandlerTestingSuite()
    {
        _userManagerMock = new Mock<UserManager<Account>>(
            new Mock<IUserStore<Account>>().Object, null, null, null, null, null, null, null, null);

        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            new Mock<IRoleStore<IdentityRole>>().Object, null, null, null, null);

        _validatorMock = new Mock<IValidator<AccountRegistrationRequest>>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _establishmentOwnerRepositoryMock = new Mock<IEstablishmentOwnerRepository>();

        _handler = new AccountRegistrationHandler(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _validatorMock.Object,
            _customerRepositoryMock.Object,
            _establishmentOwnerRepositoryMock.Object
        );

        TinyMapper.Bind<AccountRegistrationRequest, Account>(config =>
        {
            config.Bind(source: source => source.Email, target: target => target.UserName);
            config.Bind(source: source => source.Email, target: target => target.Email);
        });
    }

   [Fact(DisplayName = "Handle - Should create account and return success response when request is valid")]
    public async Task Handle_ShouldCreateAccountAndReturnSuccessResponseWhenRequestIsValid()
    {
        var request = new AccountRegistrationRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            AccountType = EAccountType.Customer
        };

        var validationResult = new ValidationResult();

        _validatorMock
            .Setup(validator => validator.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _userManagerMock
            .Setup(userManager => userManager.CreateAsync(It.IsAny<Account>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);

        _roleManagerMock
            .Setup(roleManager => roleManager.RoleExistsAsync("Customer"))
            .ReturnsAsync(true);

        _userManagerMock
            .Setup(userManager => userManager.AddToRoleAsync(It.IsAny<Account>(), "Customer"))
            .ReturnsAsync(IdentityResult.Success);

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(201, response.StatusCode);
        Assert.Equal("Account created successfully.", response.Message);

        _userManagerMock.Verify(userManager => userManager.CreateAsync(It.IsAny<Account>(), request.Password), Times.Once);
        _customerRepositoryMock.Verify(cr => cr.SaveAsync(It.IsAny<Customer>()), Times.Once);
        _userManagerMock.Verify(userManager => userManager.AddToRoleAsync(It.IsAny<Account>(), "Customer"), Times.Once);
    }

    [Fact(DisplayName = "Handle - Should throw ValidationException when request is invalid")]
    public async Task Handle_ShouldThrowValidationExceptionWhenRequestIsInvalid()
    {
        var request = new AccountRegistrationRequest
        {
            Password = "Password123!",
            AccountType = EAccountType.Customer
        };

        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new ValidationFailure("Email", "Email is required.")
        });

        _validatorMock
            .Setup(validator => validator.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        Func<Task> action = async () => await _handler.Handle(request, CancellationToken.None);

        await Assert.ThrowsAsync<ValidationException>(action);
        _userManagerMock.Verify(userManager => userManager.CreateAsync(It.IsAny<Account>(), request.Password), Times.Never);
    }

    [Fact(DisplayName = "Handle - Should register establishment owner successfully")]
    public async Task Handle_ShouldRegisterEstablishmentOwnerSuccessfully()
    {
        var request = new AccountRegistrationRequest
        {
            Email = "owner@example.com",
            Password = "Password123!",
            AccountType = EAccountType.EstablishmentOwner
        };

        var validationResult = new ValidationResult();

        _validatorMock
            .Setup(validator => validator.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _userManagerMock
            .Setup(userManager => userManager.CreateAsync(It.IsAny<Account>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);

        _roleManagerMock
            .Setup(roleManager => roleManager.RoleExistsAsync("EstablishmentOwner"))
            .ReturnsAsync(true);

        _userManagerMock
            .Setup(userManager => userManager.AddToRoleAsync(It.IsAny<Account>(), "EstablishmentOwner"))
            .ReturnsAsync(IdentityResult.Success);

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(201, response.StatusCode);
        Assert.Equal("Account created successfully.", response.Message);

        _userManagerMock
            .Verify(userManager => userManager.CreateAsync(It.IsAny<Account>(), request.Password), Times.Once);

        _establishmentOwnerRepositoryMock
            .Verify(establishmentOwnerRepository => establishmentOwnerRepository.SaveAsync(It.IsAny<EstablishmentOwner>()), Times.Once);

        _userManagerMock
            .Verify(userManager => userManager.AddToRoleAsync(It.IsAny<Account>(), "EstablishmentOwner"), Times.Once);
    }
}