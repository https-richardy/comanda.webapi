using System.IdentityModel.Tokens.Jwt;

namespace Comanda.TestingSuite.EndToEnd;

public sealed class IdentityEndpoint(WebApiFactoryFixture<Program> factory) : WebApiFixture(factory)
{
    [Fact(DisplayName = "Should register a new account")]
    public async Task ShouldRegisterANewAccount()
    {
        var client = Factory.CreateClient();
        var payload = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = "john.doe@email.com",
            Password = "JohnDoe123*"
        };

        var response = await client.PostAsJsonAsync("api/identity/register", payload);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<Response>();

        Assert.NotNull(responseContent);
        Assert.True(responseContent.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, responseContent.StatusCode);
        Assert.Equal("Account created successfully.", responseContent.Message);
    }

    [Fact(DisplayName = "Should return a 409 Conflict when registering an account with an existing email")]
    public async Task ShouldReturnBadRequestWhenRegisteringAccountWithExistingEmail()
    {
        var client = Factory.CreateClient();
        var existingEmail = "john.doe@email.com";

        var registrationRequest = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = existingEmail,
            Password = "JohnDoe1234*"
        };

        var registrationResponse = await client.PostAsJsonAsync("api/identity/register", registrationRequest);
        registrationResponse.EnsureSuccessStatusCode();


        var payload = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = existingEmail,
            Password = "JohnDoe123*"
        };

        var response = await client.PostAsJsonAsync("api/identity/register", payload);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact(DisplayName = "Should authenticate with valid credentials")]
    public async Task ShouldAuthenticateWithValidCredentials()
    {
        var client = Factory.CreateClient();

        var registrationRequest = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = "john.doe@email.com",
            Password = "JohnDoe123*"
        };

        var registrationResponse = await client.PostAsJsonAsync("api/identity/register", registrationRequest);
        registrationResponse.EnsureSuccessStatusCode();

        var authenticationRequest = new AuthenticationCredentials { Email = "john.doe@email.com", Password = "JohnDoe123*" };
        var authenticationResponse = await client.PostAsJsonAsync("api/identity/authenticate", authenticationRequest);
 
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

        var registrationRespose = await HttpClient.PostAsJsonAsync("api/identity/register", registrationRequest);
        registrationRespose.EnsureSuccessStatusCode();

        var authenticationRequest = new AuthenticationCredentials { Email = "john.doe@email.com", Password = "InvalidPassword" };
        var authenticationResponse = await HttpClient.PostAsJsonAsync("api/identity/authenticate", authenticationRequest);

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

        var response = await HttpClient.PostAsJsonAsync("api/identity/register", payload);
        var responseContent = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotEmpty(responseContent!.Errors);
    }

    [Fact(DisplayName = "Should handle password reset request and mock email sending")]
    public async Task ShouldHandlePasswordResetRequestAndMockEmailSending()
    {
        // Arrange: for reasons of DB being self-destructed at each test, we have to create a user for this scenario.
        var scopedClient = Factory.CreateClient();

        var registrationRequest = new AccountRegistrationRequest
        {
            Name = "John Doe",
            Email = "john.doe@email.com",
            Password = "JohnDoe123*"
        };

        var registrationResponse = await scopedClient.PostAsJsonAsync("api/identity/register", registrationRequest);
        registrationResponse.EnsureSuccessStatusCode();

        var emailServiceMock = new Mock<IEmailService>();

        var to = It.IsAny<string>();
        var subject = It.IsAny<string>();
        var body = It.IsAny<string>();

        emailServiceMock
            .Setup(service => service.SendEmailAsync(to, subject, body))
            .Returns(Task.CompletedTask);

        /* remove the real email service and add the mock */
        var client = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(descriptor => descriptor.ServiceType == typeof(IEmailService));
                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                services.AddSingleton<IEmailService>(emailServiceMock.Object);
            });
        }).CreateClient();

        var payload = new SendPasswordResetTokenRequest { Email = "john.doe@email.com" };
        var response = await client.PostAsJsonAsync("api/identity/request-password-reset", payload);

        Assert.True(response.IsSuccessStatusCode);
    }
}