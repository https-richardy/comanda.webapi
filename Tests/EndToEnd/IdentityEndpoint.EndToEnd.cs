using System.IdentityModel.Tokens.Jwt;

namespace Comanda.TestingSuite.EndToEnd;

public sealed class IdentityEndpointTests :
    IClassFixture<ApiIntegrationBase<Program, ComandaDbContext>>,
    IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly IFixture _fixture;
    private readonly ApiIntegrationBase<Program, ComandaDbContext> _factory;

    public IdentityEndpointTests(ApiIntegrationBase<Program, ComandaDbContext> factory)
    {
        _factory = factory;

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _httpClient = _factory.CreateClient();
    }

    [Fact(DisplayName = "Should register a new account")]
    public async Task ShouldRegisterANewAccount()
    {
        var payload = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = "john.doe@email.com",
            Password = "JohnDoe123*"
        };

        var response = await _httpClient.PostAsJsonAsync("api/identity/register", payload);
        var responseContent = await response.Content.ReadFromJsonAsync<Response>();

        response.EnsureSuccessStatusCode();

        Assert.NotNull(responseContent);
        Assert.True(responseContent.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, responseContent.StatusCode);
        Assert.Equal("Account created successfully.", responseContent.Message);
    }

    [Fact(DisplayName = "Should return a 409 Conflict when registering an account with an existing email")]
    public async Task ShouldReturnBadRequestWhenRegisteringAccountWithExistingEmail()
    {
        var existingEmail = "john.doe@email.com";

        var registrationRequest = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = existingEmail,
            Password = "JohnDoe1234*"
        };

        var registrationResponse = await _httpClient.PostAsJsonAsync("api/identity/register", registrationRequest);
        registrationResponse.EnsureSuccessStatusCode();

        var payload = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = existingEmail,
            Password = "JohnDoe123*"
        };

        var response = await _httpClient.PostAsJsonAsync("api/identity/register", payload);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact(DisplayName = "Should authenticate with valid credentials")]
    public async Task ShouldAuthenticateWithValidCredentials()
    {
        var registrationRequest = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = "john.doe@email.com",
            Password = "JohnDoe123*"
        };

        var registrationResponse = await _httpClient.PostAsJsonAsync("api/identity/register", registrationRequest);
        registrationResponse.EnsureSuccessStatusCode();

        var authenticationRequest = new AuthenticationCredentials { Email = "john.doe@email.com", Password = "JohnDoe123*" };
        var authenticationResponse = await _httpClient.PostAsJsonAsync("api/identity/authenticate", authenticationRequest);

        var responseContent = await authenticationResponse.Content.ReadFromJsonAsync<Response<AuthenticationResponse>>();

        authenticationResponse.EnsureSuccessStatusCode();

        Assert.NotNull(responseContent);
        Assert.NotNull(responseContent.Message);
        Assert.NotNull(responseContent.Data);
        Assert.True(responseContent.IsSuccess);
        Assert.False(string.IsNullOrEmpty(responseContent.Data.Token));

        var token = responseContent.Data.Token;
        Assert.NotEqual(string.Empty, token);

        var claims = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims;
        Assert.Contains(claims, claim => claim.Type == "email" && claim.Value == "john.doe@email.com");
        Assert.Contains(claims, claim => claim.Type == "role" && claim.Value == "Customer");
    }

    [Fact(DisplayName = "Should return a 401 Unauthorized when authenticating with invalid credentials")]
    public async Task ShouldReturnUnauthorizedWhenAuthenticatingWithInvalidCredentials()
    {
        const string password = "JohnDoe123*";
        var registrationRequest = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = "john.doe@email.com",
            Password = password
        };

        var registrationRespose = await _httpClient.PostAsJsonAsync("api/identity/register", registrationRequest);
        registrationRespose.EnsureSuccessStatusCode();

        var authenticationRequest = new AuthenticationCredentials { Email = "john.doe@email.com", Password = "InvalidPassword" };
        var authenticationResponse = await _httpClient.PostAsJsonAsync("api/identity/authenticate", authenticationRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, authenticationResponse.StatusCode);
    }

    [Fact(DisplayName = "Should return a 400 Bad Request when registering an account with a weak password")]
    public async Task ShouldReturnBadRequestWhenRegisteringAccountWithWeakPassword()
    {
        const string password = "weakpassword";
        var payload = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = "john.doe@email.com",
            Password = password
        };

        var response = await _httpClient.PostAsJsonAsync("api/identity/register", payload);
        var responseContent = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent.Errors);
    }

    [Fact(DisplayName = "Should handle password reset request and mock email sending")]
    public async Task ShouldHandlePasswordResetRequestAndMockEmailSending()
    {
        var registrationRequest = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = "john.doe@email.com",
            Password = "JohnDoe123*"
        };

        var registrationResponse = await _httpClient.PostAsJsonAsync("api/identity/register", registrationRequest);
        registrationResponse.EnsureSuccessStatusCode();

        var emailServiceMock = new Mock<IEmailService>();

        var to = It.IsAny<string>();
        var subject = It.IsAny<string>();
        var body = It.IsAny<string>();

        emailServiceMock
            .Setup(service => service.SendEmailAsync(to, subject, body))
            .Returns(Task.CompletedTask);

        /* remove the real email service and add the mock */
        var scopedClient = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    descriptor => descriptor.ServiceType == typeof(IEmailService));

                if (descriptor is not null)
                    services.Remove(descriptor);

                services.AddSingleton<IEmailService>(emailServiceMock.Object);
            });
        }).CreateClient();

        var payload = new SendPasswordResetTokenRequest { Email = "john.doe@email.com" };
        var response = await scopedClient.PostAsJsonAsync("api/identity/request-password-reset", payload);

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact(DisplayName = "Should handle password reset and authenticate with new password")]
    public async Task ShouldHandlePasswordResetAndAuthenticateWithNewPassword()
    {
        var registrationRequest = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = "john.doe@email.com",
            Password = "JohnDoe123*"
        };

        var registrationResponse = await _httpClient.PostAsJsonAsync("api/identity/register", registrationRequest);
        registrationResponse.EnsureSuccessStatusCode();

        var emailServiceMock = new Mock<IEmailService>();
        var confirmationTokenServiceMock = new Mock<IConfirmationTokenService>();

        emailServiceMock
            .Setup(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        confirmationTokenServiceMock
            .Setup(service => service.GenerateToken())
            .Returns(new ConfirmationToken { Token = "123456789", ExpirationDate = DateTime.UtcNow.AddHours(1) });

        // Remove the real email service and add the mock
        var scopedClient = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var emailServiceDescriptor = services.SingleOrDefault(
                    descriptor => descriptor.ServiceType == typeof(IEmailService));

                var confirmationTokenServiceDescriptor = services.SingleOrDefault(
                    descriptor => descriptor.ServiceType == typeof(IConfirmationTokenService));

                if (emailServiceDescriptor is not null &&
                    confirmationTokenServiceDescriptor is not null)
                {
                    services.Remove(emailServiceDescriptor);
                    services.Remove(confirmationTokenServiceDescriptor);
                }

                services.AddSingleton<IEmailService>(emailServiceMock.Object);
                services.AddSingleton<IConfirmationTokenService>(confirmationTokenServiceMock.Object);
            });
        }).CreateClient();

        var resetRequest = new SendPasswordResetTokenRequest
        {
            Email = "john.doe@email.com"
        };

        // Act: request password reset token
        var resetResponse = await scopedClient.PostAsJsonAsync("api/identity/request-password-reset", resetRequest);
        resetResponse.EnsureSuccessStatusCode();

        var resetToken = "123456789";

        var newPassword = "NewPassword123*";
        var resetPasswordRequest = new ResetPasswordRequest
        {
            Email = "john.doe@email.com",
            Token = resetToken,
            NewPassword = newPassword
        };

        // Act: reset password
        var passwordResetResponse = await scopedClient.PostAsJsonAsync("api/identity/reset-password", resetPasswordRequest);
        passwordResetResponse.EnsureSuccessStatusCode();

        // Act: authenticate with the new password
        var authenticationRequest = new AuthenticationCredentials
        {
            Email = "john.doe@email.com",
            Password = newPassword
        };

        var authenticationResponse = await scopedClient.PostAsJsonAsync("api/identity/authenticate", authenticationRequest);
        var responseContent = await authenticationResponse.Content.ReadFromJsonAsync<Response<AuthenticationResponse>>();

        // Assert: verify successful authentication with the new password
        authenticationResponse.EnsureSuccessStatusCode();

        Assert.NotNull(responseContent);
        Assert.NotNull(responseContent.Message);
        Assert.NotNull(responseContent.Data);
        Assert.True(responseContent.IsSuccess);
        Assert.False(string.IsNullOrEmpty(responseContent.Data.Token));

        var token = responseContent.Data.Token;

        Assert.NotEqual(string.Empty, token);

        var claims = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims;

        Assert.Contains(claims, claim => claim.Type == "email" && claim.Value == "john.doe@email.com");
        Assert.Contains(claims, claim => claim.Type == "role" && claim.Value == "Customer");
    }

    [Fact(DisplayName = "Should return a 404 Not Found when requesting password reset for a non-existent email")]
    public async Task ShouldReturnNotFoundWhenRequestingPasswordResetForNonExistentEmail()
    {
        var payload = new SendPasswordResetTokenRequest
        {
            Email = "nonexistent.email@example.com"
        };

        var response = await _httpClient.PostAsJsonAsync("api/identity/request-password-reset", payload);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Should return a 400 Bad Request when using an expired token for password reset")]
    public async Task ShouldReturnBadRequestWhenUsingExpiredTokenForPasswordReset()
    {
        // Arrange: for reasons of DB being self-destructed at each test, we have to create a user for this scenario.
        var registrationRequest = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = "john.doe@email.com",
            Password = "JohnDoe123*"
        };

        var registrationResponse = await _httpClient.PostAsJsonAsync("api/identity/register", registrationRequest);
        registrationResponse.EnsureSuccessStatusCode();

        var emailServiceMock = new Mock<IEmailService>();
        var confirmationTokenServiceMock = new Mock<IConfirmationTokenService>();

        emailServiceMock
            .Setup(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        confirmationTokenServiceMock
            .Setup(service => service.GenerateToken())
            .Returns(new ConfirmationToken { Token = "123456789", ExpirationDate = new DateTime(year: 2023, month: 09, day: 01) });

        var clientWithMocks = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var emailServiceDescriptor = services.SingleOrDefault(descriptor =>
                    descriptor.ServiceType == typeof(IEmailService));

                var confirmationTokenServiceDescriptor = services.SingleOrDefault(descriptor =>
                    descriptor.ServiceType == typeof(IConfirmationTokenService));

                if (emailServiceDescriptor is not null &&
                    confirmationTokenServiceDescriptor is not null)
                {
                    services.Remove(emailServiceDescriptor);
                    services.Remove(confirmationTokenServiceDescriptor);
                }

                services.AddSingleton<IEmailService>(emailServiceMock.Object);
                services.AddSingleton<IConfirmationTokenService>(confirmationTokenServiceMock.Object);
            });
        }).CreateClient();

        var resetRequest = new SendPasswordResetTokenRequest
        {
            Email = "john.doe@email.com"
        };

        var resetResponse = await clientWithMocks.PostAsJsonAsync("api/identity/request-password-reset", resetRequest);
        resetResponse.EnsureSuccessStatusCode();

        var expiredToken = "123456789";

        var newPassword = "NewPassword123*";
        var resetPasswordRequest = new ResetPasswordRequest
        {
            Email = "john.doe@email.com",
            Token = expiredToken,
            NewPassword = newPassword
        };

        var passwordResetResponse = await clientWithMocks.PostAsJsonAsync("api/identity/reset-password", resetPasswordRequest);

        Assert.Equal(HttpStatusCode.BadRequest, passwordResetResponse.StatusCode);
    }

    [Fact(DisplayName = "Should return a 400 Bad Request when using a weak password for password reset")]
    public async Task ShouldReturnBadRequestWhenUsingWeakPasswordForPasswordReset()
    {
        var registrationRequest = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = "john.doe@email.com",
            Password = "JohnDoe123*"
        };

        var registrationResponse = await _httpClient.PostAsJsonAsync("api/identity/register", registrationRequest);
        registrationResponse.EnsureSuccessStatusCode();

        var emailServiceMock = new Mock<IEmailService>();
        var confirmationTokenServiceMock = new Mock<IConfirmationTokenService>();

        emailServiceMock
            .Setup(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        confirmationTokenServiceMock
            .Setup(service => service.GenerateToken())
            .Returns(new ConfirmationToken { Token = "123456789", ExpirationDate = DateTime.UtcNow.AddHours(1) });

        // removes the real email service and confirmation token service and adds mocks to the service collection
        var scopedClient = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var emailServiceDescriptor = services.SingleOrDefault(descriptor =>
                    descriptor.ServiceType == typeof(IEmailService));

                var confirmationTokenServiceDescriptor = services.SingleOrDefault(descriptor =>
                    descriptor.ServiceType == typeof(IConfirmationTokenService));

                if (emailServiceDescriptor is not null &&
                    confirmationTokenServiceDescriptor is not null)
                {
                    services.Remove(emailServiceDescriptor);
                    services.Remove(confirmationTokenServiceDescriptor);
                }

                services.AddSingleton<IEmailService>(emailServiceMock.Object);
                services.AddSingleton<IConfirmationTokenService>(confirmationTokenServiceMock.Object);
            });
        }).CreateClient();

        var resetRequest = new SendPasswordResetTokenRequest
        {
            Email = "john.doe@email.com"
        };

        // Act: request password reset token
        var resetResponse = await scopedClient.PostAsJsonAsync("api/identity/request-password-reset", resetRequest);
        resetResponse.EnsureSuccessStatusCode();

        var resetToken = "123456789";

        // Act: try to reset the password using a weak password
        var weakPassword = "weak";
        var resetPasswordRequest = new ResetPasswordRequest
        {
            Email = "john.doe@email.com",
            Token = resetToken,
            NewPassword = weakPassword
        };

        var passwordResetResponse = await scopedClient.PostAsJsonAsync("api/identity/reset-password", resetPasswordRequest);
        var responseContent = await passwordResetResponse.Content.ReadFromJsonAsync<ValidationFailureResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, passwordResetResponse.StatusCode);

        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent.Errors);
        Assert.Contains(responseContent.Errors, error => error.PropertyName == "NewPassword");
    }

    [Fact(DisplayName = "Should retrieve profile information for authenticated user")]
    public async Task ShouldRetrieveProfileInformationForAuthenticatedUser()
    {
        var registrationRequest = new AccountRegistrationRequest
        {
            Name = "Jane Doe",
            Email = "jane.doe@email.com",
            Password = "JaneDoe123*"
        };

        var registrationResponse = await _httpClient.PostAsJsonAsync("api/identity/register", registrationRequest);
        registrationResponse.EnsureSuccessStatusCode();

        var authenticationRequest = new AuthenticationCredentials
        {
            Email = "jane.doe@email.com",
            Password = "JaneDoe123*"
        };

        var authenticationResponse = await _httpClient.PostAsJsonAsync("api/identity/authenticate", authenticationRequest);
        authenticationResponse.EnsureSuccessStatusCode();

        var authenticationContent = await authenticationResponse.Content.ReadFromJsonAsync<Response<AuthenticationResponse>>();

        Assert.NotNull(authenticationContent);
        Assert.NotNull(authenticationContent.Data);
        Assert.NotNull(authenticationContent.Data.Token);

        var token = authenticationContent!.Data!.Token;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var profileResponse = await _httpClient.GetAsync("api/identity/");
        profileResponse.EnsureSuccessStatusCode();

        var profileContent = await profileResponse.Content.ReadFromJsonAsync<Response<ProfileInformation>>();

        Assert.NotNull(profileContent);
        Assert.NotNull(profileContent.Data);

        Assert.True(profileContent.IsSuccess);
        Assert.Equal(StatusCodes.Status200OK, profileContent.StatusCode);
        Assert.Equal("Profile information retrieved successfully.", profileContent.Message);

        Assert.Equal("Jane Doe", profileContent.Data.Name);
        Assert.Equal("jane.doe@email.com", profileContent.Data.Email);
    }

    [Fact(DisplayName = "[RF005] - Should update account successfully when authenticated")]
    public async Task ShouldUpdateAccountSuccessfullyWhenAuthenticated()
    {
        var registrationPayload = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = "john.doe@email.com",
            Password = "JohnDoe123*"
        };

        var registrationResponse = await _httpClient.PostAsJsonAsync("api/identity/register", registrationPayload);
        registrationResponse.EnsureSuccessStatusCode();

        var authenticationRequest = new AuthenticationCredentials
        {
            Email = registrationPayload.Email,
            Password = registrationPayload.Password
        };

        var authenticationResponse = await _httpClient.PostAsJsonAsync("api/identity/authenticate", authenticationRequest);
        var authenticationContent = await authenticationResponse.Content.ReadFromJsonAsync<Response<AuthenticationResponse>>();

        authenticationResponse.EnsureSuccessStatusCode();

        Assert.NotNull(authenticationContent);
        Assert.NotNull(authenticationContent.Data);
        Assert.NotNull(authenticationContent.Data.Token);

        var token = authenticationContent.Data.Token;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updatePayload = new AccountEditingRequest
        {
            Name = "Jane Doe",
            Email = "jane.doe@email.com"
        };

        var updateResponse = await _httpClient.PutAsJsonAsync("api/identity/", updatePayload);
        updateResponse.EnsureSuccessStatusCode();

        var responseContent = await updateResponse.Content.ReadFromJsonAsync<Response>();

        Assert.NotNull(responseContent);
        Assert.True(responseContent.IsSuccess);

        Assert.Equal(StatusCodes.Status200OK, responseContent.StatusCode);
        Assert.Equal("Account updated successfully.", responseContent.Message);
    }

    [Fact(DisplayName = "[RF005] - Should return 401 Unauthorized when updating without authentication")]
    public async Task ShouldReturnUnauthorizedWhenUpdatingWithoutAuthentication()
    {
        var updatePayload = new AccountEditingRequest
        {
            Name = "Jane Doe",
            Email = "jane.doe@email.com"
        };

        var updateResponse = await _httpClient.PutAsJsonAsync("api/identity/", updatePayload);
        Assert.Equal(HttpStatusCode.Unauthorized, updateResponse.StatusCode);
    }

    [Fact(DisplayName = "[RF005] - Should return 400 Bad Request when updating with invalid data")]
    public async Task ShouldReturnBadRequestWhenUpdatingWithInvalidData()
    {
        var registrationPayload = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = "john.doe@email.com",
            Password = "JohnDoe123*"
        };

        var registrationResponse = await _httpClient.PostAsJsonAsync("api/identity/register", registrationPayload);
        registrationResponse.EnsureSuccessStatusCode();

        var authenticationRequest = new AuthenticationCredentials
        {
            Email = registrationPayload.Email,
            Password = registrationPayload.Password
        };

        var authenticationResponse = await _httpClient.PostAsJsonAsync("api/identity/authenticate", authenticationRequest);
        var authContent = await authenticationResponse.Content.ReadFromJsonAsync<Response<AuthenticationResponse>>();

        authenticationResponse.EnsureSuccessStatusCode();

        Assert.NotNull(authContent);
        Assert.NotNull(authContent.Data);
        Assert.NotNull(authContent.Data.Token);

        var token = authContent.Data.Token;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updatePayload = new AccountEditingRequest
        {
            Name = string.Empty,
            Email = string.Empty
        };

        var updateResponse = await _httpClient.PutAsJsonAsync("api/identity/", updatePayload);
        var responseContent = await updateResponse.Content.ReadFromJsonAsync<ValidationFailureResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);

        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent.Errors);
    }

    public async Task DisposeAsync() => await Task.CompletedTask;
    public async Task InitializeAsync()
    {
        _factory.CleanUp();
        await Task.CompletedTask;
    }
}