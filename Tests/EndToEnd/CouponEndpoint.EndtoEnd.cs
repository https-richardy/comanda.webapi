namespace Comanda.TestingSuite.EndToEnd;

public sealed class CouponEndpointTests :
    IClassFixture<ApiIntegrationBase<Program, ComandaDbContext>>,
    IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly IFixture _fixture;
    private readonly ApiIntegrationBase<Program, ComandaDbContext> _factory;

    public CouponEndpointTests(ApiIntegrationBase<Program, ComandaDbContext> factory)
    {
        _factory = factory;
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _httpClient = _factory.CreateClient();
    }

    [Fact(DisplayName = "Given a valid request, it must then create a new coupon")]
    public async Task GivenAValidRequestItMustThenCreateANewCoupon()
    {
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        var payload = new CouponCreationRequest
        {
            Code = "TESTCOUPONCODE",
            ExpirationDate = DateTime.UtcNow.AddDays(2),
            Type = ECouponType.Percentage,
            Discount = 10m
        };

        var response = await authenticatedClient.PostAsJsonAsync("api/coupons", payload);
        var responseContent = await response.Content.ReadFromJsonAsync<Response>();

        response.EnsureSuccessStatusCode();

        Assert.NotNull(responseContent);
        Assert.True(responseContent.IsSuccess);
    }

    [Fact(DisplayName = "Given an invalid request, it must then return a 400 bad request")]
    public async Task GivenAnInvalidRequestItMustThenReturnABadRequest()
    {
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        var payload = new CouponCreationRequest
        {
            Code = "TESTCOUPONCODE",
            ExpirationDate = DateTime.UtcNow.AddDays(-2),
            Type = ECouponType.Percentage,
            Discount = 0
        };

        var response = await authenticatedClient.PostAsJsonAsync("api/coupons", payload);
        var responseContent = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotEmpty(responseContent!.Errors);

        Assert.Contains(responseContent.Errors, error => error.PropertyName == "Discount");
        Assert.Contains(responseContent.Errors, error => error.PropertyName == "ExpirationDate");
    }

    [Fact(DisplayName = "Should return available coupons when requesting the coupon list")]
    public async Task ShouldReturnAvailableCouponsWhenRequestingTheCouponList()
    {
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        var payloads = new List<CouponCreationRequest>
        {
            new CouponCreationRequest { Code = "TESTCOUPONCODE1", ExpirationDate = DateTime.UtcNow.AddDays(2), Type = ECouponType.Percentage, Discount = 10m },
            new CouponCreationRequest { Code = "TESTCOUPONCODE2", ExpirationDate = DateTime.UtcNow.AddDays(2), Type = ECouponType.Percentage, Discount = 20m },
        };

        foreach (var payload in payloads)
        {
            var creationResponse = await authenticatedClient.PostAsJsonAsync("api/coupons", payload);
            creationResponse.EnsureSuccessStatusCode();
        }

        var response = await authenticatedClient.GetFromJsonAsync<Response<IEnumerable<Coupon>>>("api/coupons");

        Assert.NotNull(response);
        Assert.NotNull(response.Data);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
    }

    [Fact(DisplayName = "Given an invalid identifier, it must return a 404 Not Found")]
    public async Task GivenAnInvalidIdentifierItMustReturnANotFoundError()
    {
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        var response = await authenticatedClient.GetAsync("api/coupons/1");

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Given a valid identifier, it must return the corresponding coupon")]
    public async Task GivenAValidIdentifierItMustReturnTheCoupon()
    {
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        var payloads = new List<CouponCreationRequest>
        {
            new CouponCreationRequest { Code = "TESTCOUPONCODE1", ExpirationDate = DateTime.UtcNow.AddDays(2), Type = ECouponType.Percentage, Discount = 10m },
            new CouponCreationRequest { Code = "TESTCOUPONCODE2", ExpirationDate = DateTime.UtcNow.AddDays(2), Type = ECouponType.Percentage, Discount = 20m },
        };

        foreach (var payload in payloads)
        {
            var creationResponse = await authenticatedClient.PostAsJsonAsync("api/coupons", payload);
            creationResponse.EnsureSuccessStatusCode();
        }

        var response = await authenticatedClient.GetAsync("api/coupons/1");
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
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        var payload = new CouponCreationRequest
        {
            Code = "VALIDCOUPON",
            ExpirationDate = DateTime.UtcNow.AddDays(2),
            Type = ECouponType.Percentage,
            Discount = 10m
        };

        var creationResponse = await authenticatedClient.PostAsJsonAsync("api/coupons", payload);
        creationResponse.EnsureSuccessStatusCode();

        var response = await authenticatedClient.GetAsync($"api/coupons/find-by-code/{payload.Code}");
        response.EnsureSuccessStatusCode();

        var couponResponse = await response.Content.ReadFromJsonAsync<Response<Coupon>>();

        Assert.NotNull(couponResponse);
        Assert.NotNull(couponResponse.Data);
        Assert.Equal("VALIDCOUPON", couponResponse.Data.Code);
    }

    [Fact(DisplayName = "Given an invalid coupon code, it must return a 404 Not Found")]
    public async Task GivenAnInvalidCouponCodeItMustReturnNotFoundError()
    {
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        var response = await authenticatedClient.GetAsync("api/coupons/find-by-code/INVALIDCOUPON");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Given a valid request, it must successfully update the coupon")]
    public async Task GivenAValidRequestItMustSuccessfullyUpdateTheCoupon()
    {
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        var creationPayload = new CouponCreationRequest
        {
            Code = "UPDATABLECOUPON",
            ExpirationDate = DateTime.UtcNow.AddDays(2),
            Type = ECouponType.Percentage,
            Discount = 10m
        };

        var creationResponse = await authenticatedClient.PostAsJsonAsync("api/coupons", creationPayload);
        creationResponse.EnsureSuccessStatusCode();

        var updatePayload = new CouponEditingRequest
        {
            Code = "UPDATEDCOUPON",
            ExpirationDate = DateTime.UtcNow.AddDays(5),
            Type = ECouponType.Fixed,
            Discount = 20m
        };

        var updateResponse = await authenticatedClient.PutAsJsonAsync("api/coupons/1", updatePayload);
        updateResponse.EnsureSuccessStatusCode();

        var responseContent = await updateResponse.Content.ReadFromJsonAsync<Response>();

        Assert.NotNull(responseContent);
        Assert.True(responseContent.IsSuccess);
    }

    [Fact(DisplayName = "Given a non-existent coupon ID, it must return a 404 Not Found")]
    public async Task GivenANonExistentCouponIDItMustReturnNotFound()
    {
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        var updatePayload = new CouponEditingRequest
        {
            Code = "NONEXISTENTCOUPON",
            ExpirationDate = DateTime.UtcNow.AddDays(5),
            Type = ECouponType.Fixed,
            Discount = 20m
        };

        var updateResponse = await authenticatedClient.PutAsJsonAsync("api/coupons/999", updatePayload);

        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
    }

    [Fact(DisplayName = "Given an invalid request, it must return a 400 Bad Request")]
    public async Task GivenAnInvalidRequestItMustReturnBadRequest()
    {
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        var creationPayload = new CouponCreationRequest
        {
            Code = "UPDATABLECOUPON",
            ExpirationDate = DateTime.UtcNow.AddDays(2),
            Type = ECouponType.Percentage,
            Discount = 10m
        };

        var creationResponse = await authenticatedClient.PostAsJsonAsync("api/coupons", creationPayload);
        creationResponse.EnsureSuccessStatusCode();

        var updatePayload = new CouponEditingRequest
        {
            Code = "UPDATEDCOUPON",
            ExpirationDate = DateTime.UtcNow.AddDays(-5), // Invalid expiration date
            Type = ECouponType.Fixed,
            Discount = 20m
        };

        var updateResponse = await authenticatedClient.PutAsJsonAsync("api/coupons/1", updatePayload);

        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }

    public async Task InitializeAsync()
    {
        _factory.CleanUp();

        var services = _factory.GetServiceProvider();

        var userManager = services.GetService<UserManager<Account>>();
        var roleManager = services.GetService<RoleManager<IdentityRole>>();

        Assert.NotNull(userManager);
        Assert.NotNull(roleManager);

        var admin = new Account
        {
            UserName = "Comanda Administrator",
            Email = "comanda@admin.com",
        };

        var result = await userManager.CreateAsync(admin, "ComandaAdministrator123*");
        if (result.Succeeded is true)
        {
            const string adminRoleName = "Administrator";
            var adminRoleExists = await roleManager.RoleExistsAsync(adminRoleName);
            Assert.False(adminRoleExists);

            if (adminRoleExists is false)
            {
                var adminRole = new IdentityRole(adminRoleName);
                await roleManager.CreateAsync(adminRole);
            }

            await userManager.AddToRoleAsync(admin, adminRoleName);
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
