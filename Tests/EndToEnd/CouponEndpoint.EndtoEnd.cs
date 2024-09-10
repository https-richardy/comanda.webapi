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

    [Fact(DisplayName = "Should return available coupons when requesting the coupon list")]
    public async Task ShouldReturnAvailableCouponsWhenRequestingTheCouponList()
    {
        var client = GetAuthenticatedClient();
        var payloads = new List<CouponCreationRequest>
        {
            new CouponCreationRequest { Code = "TESTCOUPONCODE1", ExpirationDate = DateTime.UtcNow.AddDays(2), Type = ECouponType.Percentage, Discount = 10m },
            new CouponCreationRequest { Code = "TESTCOUPONCODE2", ExpirationDate = DateTime.UtcNow.AddDays(2), Type = ECouponType.Percentage, Discount = 20m },
        };

        foreach (var payload in payloads)
        {
            var creationResponse = await client.PostAsJsonAsync("api/coupons", payload);
            creationResponse.EnsureSuccessStatusCode();
        }

        var response = await client.GetFromJsonAsync<Response<IEnumerable<Coupon>>>("api/coupons");

        Assert.NotNull(response);
        Assert.NotNull(response.Data);

        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
    }

    [Fact(DisplayName = "Given an invalid identifier, it must return a 404 Not Found")]
    public async Task GivenAnInvalidIdentifierItMustReturnANotFoundError()
    {
        var client = GetAuthenticatedClient();
        var response = await client.GetAsync("api/coupons/1");

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Given a valid identifier, it must return the corresponding coupon")]
    public async Task GivenAValidIdentifierItMustReturnTheCoupon()
    {
        var client = GetAuthenticatedClient();
        var payloads = new List<CouponCreationRequest>
        {
            new CouponCreationRequest { Code = "TESTCOUPONCODE1", ExpirationDate = DateTime.UtcNow.AddDays(2), Type = ECouponType.Percentage, Discount = 10m },
            new CouponCreationRequest { Code = "TESTCOUPONCODE2", ExpirationDate = DateTime.UtcNow.AddDays(2), Type = ECouponType.Percentage, Discount = 20m },
        };

        foreach (var payload in payloads)
        {
            var creationResponse = await client.PostAsJsonAsync("api/coupons", payload);
            creationResponse.EnsureSuccessStatusCode();
        }

        var response = await client.GetAsync("api/coupons/1");
        response.EnsureSuccessStatusCode();

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var coupon = await response.Content.ReadFromJsonAsync<Response<Coupon>>();

        Assert.NotNull(coupon);
        Assert.NotNull(coupon.Data);

        Assert.True(coupon.IsSuccess);
        Assert.Equal("TESTCOUPONCODE1", coupon.Data.Code);
    }

    [Fact(DisplayName = "Given a valid coupon code, it must return the corresponding coupon")]
    public async Task GivenAValidCouponCodeItMustReturnTheCoupon()
    {
        var client = GetAuthenticatedClient();
        var payload = new CouponCreationRequest
        {
            Code = "VALIDCOUPON",
            ExpirationDate = DateTime.UtcNow.AddDays(2),
            Type = ECouponType.Percentage,
            Discount = 10m
        };

        var creationResponse = await client.PostAsJsonAsync("api/coupons", payload);
        creationResponse.EnsureSuccessStatusCode();

        var response = await client.GetAsync($"api/coupons/find-by-code/{payload.Code}");
        response.EnsureSuccessStatusCode();

        var couponResponse = await response.Content.ReadFromJsonAsync<Response<Coupon>>();

        Assert.NotNull(couponResponse);
        Assert.NotNull(couponResponse!.Data);
        Assert.Equal("VALIDCOUPON", couponResponse.Data.Code);
    }

    [Fact(DisplayName = "Given an invalid coupon code, it must return a 404 Not Found")]
    public async Task GivenAnInvalidCouponCodeItMustReturnNotFoundError()
    {
        var client = GetAuthenticatedClient();
        var response = await client.GetAsync("api/coupons/find-by-code/INVALIDCOUPON");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Given a valid request, it must successfully update the coupon")]
    public async Task GivenAValidRequestItMustSuccessfullyUpdateTheCoupon()
    {
        var client = GetAuthenticatedClient();

        var creationPayload = new CouponCreationRequest
        {
            Code = "UPDATABLECOUPON",
            ExpirationDate = DateTime.UtcNow.AddDays(2),
            Type = ECouponType.Percentage,
            Discount = 10m
        };
        var creationResponse = await client.PostAsJsonAsync("api/coupons", creationPayload);
        creationResponse.EnsureSuccessStatusCode();

        var updatePayload = new CouponEditingRequest
        {
            Code = "UPDATEDCOUPON",
            ExpirationDate = DateTime.UtcNow.AddDays(5),
            Type = ECouponType.Fixed,
            Discount = 20m
        };

        var updateResponse = await client.PutAsJsonAsync("api/coupons/1", updatePayload);
        updateResponse.EnsureSuccessStatusCode();

        var responseContent = await updateResponse.Content.ReadFromJsonAsync<Response>();
        Assert.NotNull(responseContent);
        Assert.True(responseContent!.IsSuccess);
    }

    [Fact(DisplayName = "Given a non-existent coupon ID, it must return a 404 Not Found")]
    public async Task GivenANonExistentCouponIDItMustReturnNotFound()
    {
        var client = GetAuthenticatedClient();

        var updatePayload = new CouponEditingRequest
        {
            Code = "NONEXISTENTCOUPON",
            ExpirationDate = DateTime.UtcNow.AddDays(5),
            Type = ECouponType.Fixed,
            Discount = 20m
        };

        var updateResponse = await client.PutAsJsonAsync("api/coupons/999", updatePayload);

        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
    }

    [Fact(DisplayName = "Given an invalid request, it must return a 400 Bad Request")]
    public async Task GivenAnInvalidRequestItMustReturnBadRequest()
    {
        var client = GetAuthenticatedClient();
        var creationPayload = new CouponCreationRequest
        {
            Code = "UPDATABLECOUPON",
            ExpirationDate = DateTime.UtcNow.AddDays(2),
            Type = ECouponType.Percentage,
            Discount = 10m
        };

        var creationResponse = await client.PostAsJsonAsync("api/coupons", creationPayload);
        creationResponse.EnsureSuccessStatusCode();

        var invalidUpdatePayload = new CouponEditingRequest
        {
            Code = "INVALIDCOUPON",
            ExpirationDate = DateTime.UtcNow,
            Type = ECouponType.Fixed,
            Discount = 0m 
        };

        var updateResponse = await client.PutAsJsonAsync("api/coupons/1", invalidUpdatePayload);

        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);

        var responseContent = await updateResponse.Content.ReadFromJsonAsync<ValidationFailureResponse>();

        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent.Errors);
        Assert.Contains(responseContent.Errors, error => error.PropertyName == "ExpirationDate");
        Assert.Contains(responseContent.Errors, error => error.PropertyName == "Discount");
    }

    [Fact(DisplayName = "Given a valid coupon ID, it must successfully delete the coupon")]
    public async Task GivenAValidCouponIDItMustSuccessfullyDeleteTheCoupon()
    {
        var client = GetAuthenticatedClient();
        var creationPayload = new CouponCreationRequest
        {
            Code = "DELETABLECOUPON",
            ExpirationDate = DateTime.UtcNow.AddDays(2),
            Type = ECouponType.Percentage,
            Discount = 10m
        };

        var creationResponse = await client.PostAsJsonAsync("api/coupons", creationPayload);
        creationResponse.EnsureSuccessStatusCode();

        var deleteResponse = await client.DeleteAsync("api/coupons/1");
        deleteResponse.EnsureSuccessStatusCode();

        var responseContent = await deleteResponse.Content.ReadFromJsonAsync<Response>();

        Assert.NotNull(responseContent);
        Assert.True(responseContent.IsSuccess);
    }

    [Fact(DisplayName = "Given a deletion request for a non-existent coupon, it must return a 404 Not Found")]
    public async Task GivenADeletionRequestForANonExistentCouponItMustReturnNotFound()
    {
        var client = GetAuthenticatedClient();
        var deleteResponse = await client.DeleteAsync("api/coupons/999");

        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    [Fact(DisplayName = "Given an unauthenticated request, it must return a 401 Unauthorized")]
    public async Task GivenAnUnauthenticatedRequestItMustReturnUnauthorized()
    {
        var deleteResponse = await HttpClient.DeleteAsync("api/coupons/1");

        Assert.Equal(HttpStatusCode.Unauthorized, deleteResponse.StatusCode);
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