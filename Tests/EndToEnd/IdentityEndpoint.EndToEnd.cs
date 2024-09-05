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
}