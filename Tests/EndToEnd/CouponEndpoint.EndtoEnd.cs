namespace Comanda.TestingSuite.EndToEnd;

public sealed class CouponEndpointTests : WebApiFixture
{
    private string _bearerToken = string.Empty;
    private HttpClient _authenticatedClient = null!;
    private readonly IFixture _fixture;

    public CouponEndpointTests(WebApiFactoryFixture<Program> factory) : base(factory)
    {
        AuthenticateAdminUserAsync().GetAwaiter().GetResult();

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact(DisplayName = "Given a valid request, it must then create a new coupon")]
    public async Task GivenAValidRequestItMustThenCreateANewCoupon()
    {
        var client = GetAuthenticatedClient();
        var payload = new CouponCreationRequest
        {
            Code = "TESTCOUPONCODE",
            ExpirationDate = DateTime.UtcNow.AddDays(2),
            Type = ECouponType.Percentage,
            Discount = 10m
        };

        var response = await client.PostAsJsonAsync("api/coupons", payload);
        var responseContent = await response.Content.ReadFromJsonAsync<Response>();


        response.EnsureSuccessStatusCode();

        Assert.NotNull(responseContent);
        Assert.True(responseContent.IsSuccess);
    }

    [Fact(DisplayName = "Given an invalid request, it must then return a 400 bad request")]
    public async Task GivenAnInvalidRequestItMustThenReturnABadRequest()
    {
        var client = GetAuthenticatedClient();
        var payload = new CouponCreationRequest
        {
            Code = "TESTCOUPONCODE",
            ExpirationDate = DateTime.UtcNow.AddDays(-2),
            Type = ECouponType.Percentage,
            Discount = 0
        };

        var response = await client.PostAsJsonAsync("api/coupons", payload);
        var responseContent = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotEmpty(responseContent!.Errors);

        Assert.Contains(responseContent.Errors, error => error.PropertyName == "Discount");
        Assert.Contains(responseContent!.Errors, error => error.PropertyName == "ExpirationDate");
    }

    private async Task AuthenticateAdminUserAsync()
    {
        var payload = new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "Ri34067294*"
        };

        var response = await HttpClient.PostAsJsonAsync("api/identity/authenticate", payload);
        var responseContent = await response.Content.ReadFromJsonAsync<Response<AuthenticationResponse>>();

        response.EnsureSuccessStatusCode();
        _bearerToken = responseContent!.Data!.Token ?? throw new Exception("authentication failed");

        _authenticatedClient = Factory.CreateClient();
        _authenticatedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
    }

    private HttpClient GetAuthenticatedClient()
    {
        if (_authenticatedClient is null)
            throw new Exception("Authenticated client not initialized.");

        return _authenticatedClient;
    }
}