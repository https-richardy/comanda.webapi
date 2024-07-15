namespace Comanda.TestingSuite.HandlersTestSuite.IdentityHandlers;

public sealed class AccountRegistrationHandlerTest
{
    private readonly Mock<UserManager<Account>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly Mock<IValidator<AccountRegistrationRequest>> _validatorMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly IRequestHandler<AccountRegistrationRequest, Response> _handler;
    private readonly IServiceCollection _services;
    private readonly IFixture _fixture;

    public AccountRegistrationHandlerTest()
    {
        #pragma warning disable CS8625 // disable CS8625 because of Mocks they need to be null.
        #region Mocking
        _userManagerMock = new Mock<UserManager<Account>>(
            Mock.Of<IUserStore<Account>>(),
            null, /* passwordHasher */
            null, /* userValidators */
            null, /* passwordValidators */
            null, /* keyNormalizer */
            null, /* errors */
            null, /* services */
            null, /* logger */
            null  /* contextAccessor */
        );

        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            Mock.Of<IRoleStore<IdentityRole>>(),
            null, /* roleValidators */
            null, /* keyNormalizer */
            null, /* errors */
            null  /* logger */
        );

        _customerRepositoryMock = new Mock<ICustomerRepository>();

        _validatorMock = new Mock<IValidator<AccountRegistrationRequest>>();
        #endregion

        _services = new ServiceCollection();
        _services.AddMapping();

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _handler = new AccountRegistrationHandler(
            userManager: _userManagerMock.Object,
            roleManager: _roleManagerMock.Object,
            customerRepository: _customerRepositoryMock.Object,
            validator: _validatorMock.Object
        );
    }

    [Fact(DisplayName = "Given a valid request, should return success response")]
    public async Task GivenValidRequest_ShouldReturnSuccessResponse()
    {
        var request = _fixture.Create<AccountRegistrationRequest>();

        #region setup the behavior of mocks in this scenario. 
        _validatorMock.Setup(validator => validator.ValidateAsync(
                It.IsAny<AccountRegistrationRequest>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        _userManagerMock.Setup(userManager => userManager.CreateAsync(
                It.IsAny<Account>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(IdentityResult.Success);
        #endregion

        var result = await _handler.Handle(request, CancellationToken.None);

        /* checking if the handler called the validator passing an AccountRegistrationRequest. */
        _validatorMock.Verify(validator => validator.ValidateAsync(
            It.IsAny<AccountRegistrationRequest>(),
            It.IsAny<CancellationToken>()
        ));

        /* checking if the handler called the userManager passing an Account and a password. */
        _userManagerMock.Verify(userManager => userManager.CreateAsync(
            It.IsAny<Account>(),
            It.IsAny<string>()
        ));

        /* checking if the handler called the userManager passing an Account and a role. */
        _userManagerMock.Verify(userManager => userManager.AddToRoleAsync(
            It.IsAny<Account>(),
            It.IsAny<string>()
        ));

        Assert.NotNull(result);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal("Account created successfully.", result.Message);
    }

    [Fact(DisplayName = "Given an invalid request, should return error response")]
    public async Task GivenInvalidRequest_ShouldReturnErrorResponse()
    {
        var request = _fixture.Create<AccountRegistrationRequest>();

        _validatorMock.Setup(validator => validator.ValidateAsync(
                It.IsAny<AccountRegistrationRequest>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult
            {
                Errors = new List<FluentValidation.Results.ValidationFailure>
                {
                    new("Email", "Invalid email address format.")
                }
            });

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact(DisplayName = "Given a valid request, should save customer details")]
    public async Task GivenValidRequest_ShouldSaveCustomerDetails()
    {
        var request = _fixture.Create<AccountRegistrationRequest>();

        _validatorMock.Setup(validator => validator.ValidateAsync(
                It.IsAny<AccountRegistrationRequest>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        _userManagerMock.Setup(userManager => userManager.CreateAsync(
                It.IsAny<Account>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(IdentityResult.Success);

        await _handler.Handle(request, CancellationToken.None);

        _customerRepositoryMock.Verify(repository => repository.SaveAsync(
            It.Is<Customer>(customer => customer.FullName == request.Name)
        ), Times.Once);
    }

    [Fact(DisplayName = "Given a request, should add user to 'Customer' role if it doesn't exist")]
    public async Task GivenRequest_ShouldAddUserToCustomerRoleIfNotExists()
    {
        var request = _fixture.Create<AccountRegistrationRequest>();

        _validatorMock.Setup(validator => validator.ValidateAsync(
                It.IsAny<AccountRegistrationRequest>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new ValidationResult());

        _userManagerMock.Setup(userManager => userManager.CreateAsync(
                It.IsAny<Account>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(IdentityResult.Success);

        _roleManagerMock.Setup(roleManager => roleManager.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        await _handler.Handle(request, CancellationToken.None);

        _roleManagerMock.Verify(roleManager => roleManager.CreateAsync(
            It.Is<IdentityRole>(role => role.Name == "Customer")
        ), Times.Once);

        _userManagerMock.Verify(userManager => userManager.AddToRoleAsync(
            It.IsAny<Account>(), "Customer"
        ), Times.Once);
    }
}