namespace Comanda.TestingSuite.EndToEnd;

public sealed class OrderEndpointTests :
    IClassFixture<ApiIntegrationBase<Program, ComandaDbContext>>,
    IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly ApiIntegrationBase<Program, ComandaDbContext> _factory;
    private readonly IFixture _fixture;

    public OrderEndpointTests(ApiIntegrationBase<Program, ComandaDbContext> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact(DisplayName = "Given existing orders, when fetching orders, then it must return a list of orders")]
    public async Task GivenExistingOrdersWhenFetchingOrdersThenItMustReturnListOfOrders()
    {
        // arrange: obtaining the necessary services
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: creating some orders
        var orders = _fixture.Build<Order>()
            .With(order => order.Status, EOrderStatus.Pending)
            .CreateMany(10)
            .ToList();

        await dbContext.Orders.AddRangeAsync(orders);
        await dbContext.SaveChangesAsync();

        var retrievedOrders = await dbContext.Orders.ToListAsync();

        Assert.NotNull(retrievedOrders);
        Assert.NotEmpty(retrievedOrders);

        // arrange: authenticate httpClient as administrator
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: send a request to obtain the orders with query parameters
        var queryParams = new Dictionary<string, string>
        {
            { "status", "0" },
            { "pageNumber", "1" },
            { "pageSize", "10" }
        };

        var urlEncodedContent = new FormUrlEncodedContent(queryParams);
        var queryString = await urlEncodedContent.ReadAsStringAsync();

        var response = await authenticatedClient.GetAsync($"api/orders?{queryString}");
        var content = await response.Content.ReadFromJsonAsync<Response<PaginationHelper<FormattedOrder>>>();

        response.EnsureSuccessStatusCode();

        Assert.NotNull(response);
        Assert.NotNull(content);
        Assert.NotNull(content.Data);

        Assert.NotNull(content.Data.Results);
        Assert.NotEmpty(content.Data.Results);
        Assert.Equal(10, content.Data.Results.Count());
    }

    [Fact(DisplayName = "Should not allow anonymous and authenticated customers to retrieve orders")]
    public async Task ShouldNotAllowAnonymousAndAuthenticatedCustomersToRetrieveOrders()
    {
        // arrange: authenticate httpClient as customer
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "john.doe@email.com",
            Password = "JohnDoe1234*"
        });

        // act and assert: send a request to obtain the orders with query parameters
        var queryParams = new Dictionary<string, string>
        {
            { "status", "0" },
            { "pageNumber", "1" },
            { "pageSize", "10" }
        };

        var urlEncodedContent = new FormUrlEncodedContent(queryParams);
        var queryString = await urlEncodedContent.ReadAsStringAsync();

        var response = await authenticatedClient.GetAsync($"api/orders?{queryString}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        // arrange: set httpClient to be anonymous
        var anonymousClient = _factory.CreateClient();

        // act and assert: send a request to obtain the orders with query parameters

        response = await anonymousClient.GetAsync($"api/orders?{queryString}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "Given a valid order ID, when fetching order details, then it must return the corresponding order")]
    public async Task GivenAValidOrderIdWhenFetchingOrderDetailsThenItMustReturnTheCorrespondingOrder()
    {
        // arrange: obtaining the necessary services
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: creating an order
        var order = _fixture.Create<Order>();

        await dbContext.Orders.AddAsync(order);
        await dbContext.SaveChangesAsync();

        // arrange: authenticate httpClient as administrator
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        var response = await authenticatedClient.GetAsync($"api/orders/{order.Id}");
        var content = await response.Content.ReadFromJsonAsync<Response<FormattedOrderDetails>>();

        response.EnsureSuccessStatusCode();

        Assert.NotNull(content);
        Assert.NotNull(content.Data);

        Assert.Equal(order.Id, content.Data.Id);
        Assert.Equal(order.Status, content.Data.Status);
        Assert.Equal(order.Total, content.Data.Total);
        Assert.Equal(order.Date, content.Data.Date);
        Assert.Equal(order.CustomerName, content.Data.Customer);
        Assert.Equal(order.Items.Count, content.Data.Items.Count);

        Assert.NotNull(content.Data.Items);
        Assert.NotEmpty(content.Data.Items);

        Assert.NotNull(content.Data.ShippingAddress);
        Assert.False(string.IsNullOrWhiteSpace(content.Data.ShippingAddress));
    }

    [Fact(DisplayName = "Should not allow anonymous and authenticated customers to retrieve order details")]
    public async Task ShouldNotAllowAnonymousAndAuthenticatedCustomersToRetrieveOrderDetails()
    {
        // arrange: authenticate httpClient as customer
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "john.doe@email.com",
            Password = "JohnDoe1234*"
        });

        // act and assert: send a request to obtain the order details
        var response = await authenticatedClient.GetAsync($"api/orders/999");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        // arrange: set httpClient to be anonymous
        var anonymousClient = _factory.CreateClient();

        // act and assert: send a request to obtain the order details
        response = await anonymousClient.GetAsync($"api/orders/999");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "Given a valid order ID, when fetching order details with a non-existing ID, then it must return 404")]
    public async Task GivenAValidOrderIdWhenFetchingOrderDetailsWithNonExistingIdThenItMustReturn404()
    {
        // arrange: obtaining the necessary services
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: creating an order
        var order = _fixture.Create<Order>();

        await dbContext.Orders.AddAsync(order);
        await dbContext.SaveChangesAsync();

        // arrange: authenticate httpClient as administrator
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: fetch order details with a non-existing ID
        var nonExistingOrderId = 999;
        var response = await authenticatedClient.GetAsync($"api/orders/{nonExistingOrderId}");

        // assert: verify that the response status code is 404
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Given a valid request to set order status, when processing the request, then it must update the order status")]
    public async Task GivenAValidRequestToSetOrderStatusWhenProcessingTheRequestThenItMustUpdateTheOrderStatus()
    {
        // arrange: obtaining the necessary services
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: creating an order
        var order = _fixture.Build<Order>()
            .With(order => order.Status, EOrderStatus.Pending)
            .Create();

        await dbContext.Orders.AddAsync(order);
        await dbContext.SaveChangesAsync();

        // arrange: authenticate httpClient as administrator
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        var payload = new SetOrderStatusRequest
        {
            Status = EOrderStatus.Confirmed
        };

        // act: send a request to change the order status.

        var response = await authenticatedClient.PutAsJsonAsync($"api/orders/{order.Id}/set-status", payload);
        response.EnsureSuccessStatusCode();

        var updateResponse = await response.Content.ReadFromJsonAsync<Response>();

        // assert: verify that the response was successful

        Assert.NotNull(updateResponse);
        Assert.True(updateResponse.IsSuccess);

        // assert: verify that the order status has been updated
        var updatedOrder = await dbContext.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(order => order.Id == order.Id);

        Assert.NotNull(updatedOrder);
        Assert.Equal(EOrderStatus.Confirmed, updatedOrder.Status);
    }

    [Fact(DisplayName = "Should not allow authenticated customers to set order status")]
    public async Task ShouldNotAllowAuthenticatedCustomersToSetOrderStatus()
    {
        // arrange: obtaining the necessary services
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: authenticate httpClient as customer
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "john.doe@email.com",
            Password = "JohnDoe1234*"
        });

        // arrange: create a sample order
        var order = _fixture.Create<Order>();

        await dbContext.Orders.AddAsync(order);
        await dbContext.SaveChangesAsync();

        var payload = new SetOrderStatusRequest
        {
            Status = EOrderStatus.Confirmed
        };

        // act and assert: send a request to change the order status
        var response = await authenticatedClient.PutAsJsonAsync($"api/orders/{order.Id}/set-status", payload);
        
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "Should not allow anonymous users to set order status")]
    public async Task ShouldNotAllowAnonymousUsersToSetOrderStatus()
    {
        // arrange: obtaining the necessary services
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: create a sample order
        var order = _fixture.Create<Order>();

        await dbContext.Orders.AddAsync(order);
        await dbContext.SaveChangesAsync();

        var payload = new SetOrderStatusRequest
        {
            Status = EOrderStatus.Confirmed
        };

        // arrange: set httpClient to be anonymous
        var anonymousClient = _factory.CreateClient();

        // act and assert: send a request to change the order status
        var response = await anonymousClient.PutAsJsonAsync($"api/orders/{order.Id}/set-status", payload);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "Customer should be able to cancel their own order")]
    public async Task CustomerShouldBeAbleToCancelTheirOwnOrder()
    {
        // arrange: obtaining the necessary services
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: creating a order and authenticate the customer
        var order = _fixture.Build<Order>()
            .With(order => order.Status, EOrderStatus.Pending)
            .Create();

        await dbContext.Orders.AddAsync(order);
        await dbContext.SaveChangesAsync();

        // arrange: mock the refund manager

        var refundManagerMock = new Mock<IRefundManager>();
        refundManagerMock
            .Setup(manager => manager.RefundOrderAsync(It.IsAny<Order>()))
            .ReturnsAsync(new Stripe.Refund { Status = "succeeded" });

        // arrange: we are removing the real refund manager service so that this test does not depend on internet connection
        // and also we don't want to perform real payments in test mode on stripe or something like that
        var scopedClient = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var refundManagerDescriptor = services.SingleOrDefault(descriptor =>
                    descriptor.ServiceType == typeof(IRefundManager));

                if (refundManagerDescriptor is not null)
                    services.Remove(refundManagerDescriptor);

                services.AddScoped(provider => refundManagerMock.Object);
            });
        })
        .CreateClient();

        // arrange: authenticate httpClient as customer

        var authenticationResponse = await scopedClient.PostAsJsonAsync($"api/identity/authenticate", new AuthenticationCredentials
        {
            Email = "john.doe@email.com",
            Password = "JohnDoe1234*"
        });

        var authenticationContent = await authenticationResponse.Content.ReadFromJsonAsync<Response<AuthenticationResponse>>();
        authenticationResponse.EnsureSuccessStatusCode();

        Assert.NotNull(authenticationContent);
        Assert.NotNull(authenticationContent.Data);
        Assert.NotNull(authenticationContent.Data.Token);

        var authorizationHeader = new AuthenticationHeaderValue("Bearer", authenticationContent.Data.Token);
        scopedClient.DefaultRequestHeaders.Authorization = authorizationHeader;

        // act: send a request to cancel the order
        var response = await scopedClient.PostAsJsonAsync($"api/orders/{order.Id}/cancel", new OrderCancellationRequest());
        var content = await response.Content.ReadFromJsonAsync<Response>();

        // assert: verify that the response was successful
        response.EnsureSuccessStatusCode();

        Assert.NotNull(content);
        Assert.NotNull(content.Message);

        // assert: verify that the order status has been updated
        var updatedOrder = await dbContext.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(order => order.Id == order.Id);

        Assert.NotNull(updatedOrder);
        Assert.Equal(EOrderStatus.CancelledByCustomer, updatedOrder.Status);
    }

    [Fact(DisplayName = "Administrator should be able to cancel any order")]
    public async Task AdministratorShouldBeAbleToCancelAnyOrder()
    {
        // arrange: obtaining the necessary services
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: creating a order and authenticate the customer
        var order = _fixture.Build<Order>()
            .With(order => order.Status, EOrderStatus.Pending)
            .Create();

        await dbContext.Orders.AddAsync(order);
        await dbContext.SaveChangesAsync();

        // arrange: mock the refund manager

        var refundManagerMock = new Mock<IRefundManager>();
        refundManagerMock
            .Setup(manager => manager.RefundOrderAsync(It.IsAny<Order>()))
            .ReturnsAsync(new Stripe.Refund { Status = "succeeded" });

        // arrange: we are removing the real refund manager service so that this test does not depend on internet connection
        // and also we don't want to perform real payments in test mode on stripe or something like that
        var scopedClient = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var refundManagerDescriptor = services.SingleOrDefault(descriptor =>
                    descriptor.ServiceType == typeof(IRefundManager));

                if (refundManagerDescriptor is not null)
                    services.Remove(refundManagerDescriptor);

                services.AddScoped(provider => refundManagerMock.Object);
            });
        })
        .CreateClient();

        // arrange: authenticate httpClient as administrator

        var authenticationResponse = await scopedClient.PostAsJsonAsync($"api/identity/authenticate", new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        var authenticationContent = await authenticationResponse.Content.ReadFromJsonAsync<Response<AuthenticationResponse>>();
        authenticationResponse.EnsureSuccessStatusCode();

        Assert.NotNull(authenticationContent);
        Assert.NotNull(authenticationContent.Data);
        Assert.NotNull(authenticationContent.Data.Token);

        var authorizationHeader = new AuthenticationHeaderValue("Bearer", authenticationContent.Data.Token);
        scopedClient.DefaultRequestHeaders.Authorization = authorizationHeader;

        // act: send a request to cancel the order
        var response = await scopedClient.PostAsJsonAsync($"api/orders/{order.Id}/cancel", new OrderCancellationRequest());
        var content = await response.Content.ReadFromJsonAsync<Response>();

        // assert: verify that the response was successful
        response.EnsureSuccessStatusCode();

        Assert.NotNull(content);
        Assert.NotNull(content.Message);

        // assert: verify that the order status has been updated
        var updatedOrder = await dbContext.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(order => order.Id == order.Id);

        Assert.NotNull(updatedOrder);
        Assert.Equal(EOrderStatus.CancelledBySystem, updatedOrder.Status);
    }

    [Fact(DisplayName = "Anonymous user should not be able to cancel an order")]
    public async Task AnonymousUserShouldNotBeAbleToCancelAnOrder()
    {
        // arrange: obtaining the necessary services
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: creating a order
        var order = _fixture.Build<Order>()
            .With(o => o.Status, EOrderStatus.Pending)
            .Create();

        await dbContext.Orders.AddAsync(order);
        await dbContext.SaveChangesAsync();

        // act: sends an unauthenticated request to cancel an existing request.
        var anonymousClient = _factory.CreateClient();
        var response = await anonymousClient.PostAsJsonAsync($"api/orders/{order.Id}/cancel", new OrderCancellationRequest());

        // assert: checks if the server response is “unauthorized”
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory(DisplayName = "User should not be able to cancel a delivered or shipped order")]
    [InlineData(EOrderStatus.Delivered)]
    [InlineData(EOrderStatus.Shipped)]
    public async Task UserShouldNotBeAbleToCancelDeliveredOrShippedOrder(EOrderStatus status)
    {
        // arrange: obtaining the necessary services
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: creating a order
        var order = _fixture.Build<Order>()
            .With(order => order.Status, status)
            .Create();

        await dbContext.Orders.AddAsync(order);
        await dbContext.SaveChangesAsync();

        // arrange: authenticate httpClient as a customer or administrator
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "john.doe@email.com",
            Password = "JohnDoe1234*"
        });

        // act: send a request to cancel the order
        var response = await authenticatedClient.PostAsJsonAsync($"api/orders/{order.Id}/cancel", new OrderCancellationRequest());
        var content = await response.Content.ReadFromJsonAsync<Response>();

        // assert: verify that the response indicates a bad request
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        Assert.NotNull(content);
        Assert.NotNull(content.Message);
    }

    [Fact(DisplayName = "User should receive a not found response when trying to cancel a non-existent order")]
    public async Task UserShouldReceiveNotFoundWhenCancellingNonExistentOrder()
    {
        // arrange: authenticate httpClient as a customer or administrator
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "john.doe@email.com",
            Password = "JohnDoe1234*"
        });

        var invalidOrderId = 999;

        // act: send a request to cancel the non-existent order
        var response = await authenticatedClient.PostAsJsonAsync($"api/orders/{invalidOrderId}/cancel", new OrderCancellationRequest());
        var content = await response.Content.ReadFromJsonAsync<Response>();

        // assert: verify that the response indicates not found
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        Assert.NotNull(content);
        Assert.NotNull(content.Message);
    }

    public async Task DisposeAsync() => await Task.CompletedTask;
    public async Task InitializeAsync()
    {
        _factory.CleanUp();

        # region creating an administrator for each test

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

        #endregion

        #region creating an customer for each test

        var signupCredentials = new AccountRegistrationRequest
        {
            Email = "john.doe@email.com",
            Name = "John Doe",
            Password = "JohnDoe1234*"
        };

        var response = await _httpClient.PostAsJsonAsync("api/identity/register", signupCredentials);
        response.EnsureSuccessStatusCode();

        #endregion
    }
}