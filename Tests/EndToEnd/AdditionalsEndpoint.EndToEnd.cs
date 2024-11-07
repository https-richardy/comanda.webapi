namespace Comanda.TestingSuite.EndToEnd;

public sealed class AdditionalsEndpointEndToEndTestSuite :
    IClassFixture<ApiIntegrationBase<Program, ComandaDbContext>>,
    IAsyncLifetime
{
    private readonly ApiIntegrationBase<Program, ComandaDbContext> _factory;
    private readonly IFixture _fixture;
    private readonly HttpClient _httpClient;

    public AdditionalsEndpointEndToEndTestSuite(ApiIntegrationBase<Program, ComandaDbContext> factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact(DisplayName = "Given a request to the additionals listing endpoint, it should return a successful response")]
    public async Task GivenARequestToTheAdditionalsEndpoint_WhenTheRequestIsSent_ThenASuccessfulResponseShouldBeReturned()
    {
        // arrange: obtaining the necessary services

        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: creating some additionals
        const int additionalQuantity = 5;

        var category = _fixture.Create<Category>();

        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        var additionals = _fixture.Build<Additional>()
            .With(additional => additional.Category, category)
            .CreateMany(additionalQuantity);

        await dbContext.Additionals.AddRangeAsync(additionals);
        await dbContext.SaveChangesAsync();

        // arrange: authenticate httpClient as administrator
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: fetch additionals
        var response = await authenticatedClient.GetAsync("api/additionals");
        var content = await response.Content.ReadFromJsonAsync<Response<IEnumerable<Additional>>>();

        response.EnsureSuccessStatusCode();

        Assert.NotNull(content);
        Assert.NotNull(content.Data);

        Assert.Equal(additionalQuantity, content.Data.Count());

        Assert.All(content.Data, additional =>
        {
            Assert.Equal(additional.Category.Id, category.Id);
            Assert.True(additional.Price > 0);
            Assert.False(string.IsNullOrEmpty(additional.Name));
        });
    }

    [Fact(DisplayName = "Given a category, when requesting the additionals listing, it should return the available additionals for that category")]
    public async Task GivenACategory_WhenRequestingAdditionalsListing_ThenAvailableAdditionalsShouldBeReturned()
    {
        // arrange: obtaining the necessary services
        var services = _factory.GetServiceProvider();
        var dbContext = _factory.GetDbContext();

        // arrange: creating categories and additionals
        const int additionalQuantity = 5;

        var otherCategory = _fixture.Create<Category>();
        var snackCategory = _fixture.Build<Category>()
            .With(category => category.Name, "snack")
            .Create();

        var categories = new List<Category> { snackCategory, otherCategory };

        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();

        var snackAdditionals = _fixture.Build<Additional>()
            .With(additional => additional.Category, snackCategory)
            .CreateMany(additionalQuantity)
            .ToList();

        var otherAdditionals = _fixture.Build<Additional>()
            .With(additional => additional.Category, otherCategory)
            .CreateMany(10)
            .ToList();

        var additionals = snackAdditionals
            .Concat(otherAdditionals)
            .ToList();

        await dbContext.Additionals.AddRangeAsync(additionals);
        await dbContext.SaveChangesAsync();

        // arrange: authenticate httpClient as administrator
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: fetching additionals for the snack category
        var queryParams = new Dictionary<string, string>
        {
            { "categoryId", $"{snackCategory.Id}" },
        };

        var urlEncodedContent = new FormUrlEncodedContent(queryParams);
        var queryString = await urlEncodedContent.ReadAsStringAsync();

        var response = await authenticatedClient.GetFromJsonAsync<Response<IEnumerable<Additional>>>($"api/additionals/search?{queryString}");

        // assert: verifying the response and data
        Assert.NotNull(response);
        Assert.NotNull(response.Data);
        Assert.NotEmpty(response.Data);
        Assert.Equal(additionalQuantity, response.Data.Count());
    }

    [Fact(DisplayName = "Given an invalid category, when requesting the additionals listing, it should return 404 Not Found")]
    public async Task GivenAnInvalidCategory_WhenRequestingAdditionalsListing_ThenNotFoundShouldBeReturned()
    {
        // arrange: obtaining the necessary services
        var dbContext = _factory.GetDbContext();

        // arrange: creating valid categories without the requested category
        var existingCategory = _fixture.Create<Category>();

        await dbContext.Categories.AddAsync(existingCategory);
        await dbContext.SaveChangesAsync();

        // arrange: authenticate httpClient as administrator
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: requesting additionals with a non-existent categoryId
        var queryParams = new Dictionary<string, string>
        {
            { "categoryId", "999" },
        };

        var urlEncodedContent = new FormUrlEncodedContent(queryParams);
        var queryString = await urlEncodedContent.ReadAsStringAsync();

        var response = await authenticatedClient.GetAsync($"api/additionals/search?{queryString}");

        // assert: verifying the response for 404 Not Found
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Given valid data, when creating an additional, it should return 201 Created")]
    public async Task GivenValidData_WhenCreatingAnAdditional_ThenItShouldReturnCreated()
    {
        // arrange: configure DbContext and authentication
        var dbContext = _factory.GetDbContext();
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // arrange: create an existing category
        var category = _fixture.Create<Category>();

        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        // act: send request to create an additional
        var createRequest = _fixture.Build<AdditionalCreationRequest>()
            .With(payload => payload.CategoryId, category.Id)
            .Create();

        var response = await authenticatedClient.PostAsJsonAsync("/api/additionals", createRequest);
        var createdAdditional = await dbContext.Additionals.FirstAsync();

        // assert: verify successful response

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(createdAdditional);
    }

    [Fact(DisplayName = "Given a non-existent category, when creating an additional, it should return 404 not found")]
    public async Task GivenNonExistentCategory_WhenCreatingAnAdditional_ThenItShouldReturnNotFound()
    {
        // arrange: configure authentication
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: try to create an additional with a non-existent category
        var createRequest = new AdditionalCreationRequest
        {
            Name = "Cheddar",
            Price = 2.50m,
            CategoryId = 999
        };

        var response = await authenticatedClient.PostAsJsonAsync("/api/additionals", createRequest);

        // assert: verify that the status code is 404 not found
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Given invalid data, when creating an additional, it should return 400 Bad Request")]
    public async Task GivenInvalidData_WhenCreatingAnAdditional_ThenItShouldReturnBadRequest()
    {
        // arrange: configure authentication
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: try to create an additional with invalid data (no name)
        var createRequest = new AdditionalCreationRequest
        {
            Name = "",
            Price = -1m,
            CategoryId = 999
        };

        var response = await authenticatedClient.PostAsJsonAsync("/api/additionals", createRequest);

        // assert: verify that the status code is 400 Bad Request
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Given valid data, when updating an additional, it should return 200 OK and update the additional")]
    public async Task GivenValidData_WhenUpdatingAnAdditional_ThenItShouldReturnOkAndUpdateTheAdditional()
    {
        // arrange: configure DbContext and authentication
        var dbContext = _factory.GetDbContext();
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // arrange: create an existing category
        var category = _fixture.Create<Category>();

        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        // arrange: create an existing additional
        var additional = _fixture.Build<Additional>()
            .With(additional => additional.Category, category)
            .Create();

        await dbContext.Additionals.AddAsync(additional);
        await dbContext.SaveChangesAsync();

        // act: prepare the update request
        var updateRequest = new AdditionalEditingRequest
        {
            Name = "Updated Cheddar",
            Price = 3.00m,
            CategoryId = category.Id
        };

        var response = await authenticatedClient.PutAsJsonAsync($"api/additionals/{additional.Id}", updateRequest);

        // assert: verify successful response and that the additional was updated
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var httpResponse = await authenticatedClient.GetAsync("api/additionals");
        var responseContent = await httpResponse.Content.ReadFromJsonAsync<Response<IEnumerable<Additional>>>();

        Assert.NotNull(responseContent);
        Assert.NotNull(responseContent.Data);

        var updatedAdditional = responseContent.Data.First(additional => additional.Id == additional.Id);

        Assert.Equal("Updated Cheddar", updatedAdditional.Name);
        Assert.Equal(3.00m, updatedAdditional.Price);
        Assert.Equal(category.Id, updatedAdditional.Category.Id);
    }

    [Fact(DisplayName = "Given a non-existent category, when updating an additional, it should return 404 Not Found")]
    public async Task GivenNonExistentCategory_WhenUpdatingAnAdditional_ThenItShouldReturnNotFound()
    {
        // arrange: obtaining the necessary services
        var dbContext = _factory.GetDbContext();

        // arrange: configure authentication
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // arrange: create an existing additional
        var category = _fixture.Create<Category>();

        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        var additional = _fixture.Build<Additional>()
            .With(additional => additional.Category, category)
            .Create();

        await dbContext.Additionals.AddAsync(additional);
        await dbContext.SaveChangesAsync();

        // act: try to update with a non-existent category
        var updateRequest = new AdditionalEditingRequest
        {
            AdditionalId = additional.Id,
            Name = "Updated Cheddar",
            Price = 3.00m,
            CategoryId = 999
        };

        var response = await authenticatedClient.PutAsJsonAsync($"api/additionals/{additional.Id}", updateRequest);

        // assert: verify 404 Not Found
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Given a non-existent additional, when updating an additional, it should return 404 Not Found")]
    public async Task GivenNonExistentAdditional_WhenUpdatingAnAdditional_ThenItShouldReturnNotFound()
    {
        // arrange: obtaining the necessary services
        var dbContext = _factory.GetDbContext();

        // arrange: configure authentication
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // arrange: create an existing category
        var category = _fixture.Create<Category>();

        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        // act: try to update a non-existent additional
        var updateRequest = new AdditionalEditingRequest
        {
            AdditionalId = 999,
            Name = "Updated Cheddar",
            Price = 3.00m,
            CategoryId = category.Id
        };

        var response = await authenticatedClient.PutAsJsonAsync($"/api/additionals/999", updateRequest);

        // assert: verify 404 Not Found
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Given invalid data, when updating an additional, it should return 400 Bad Request")]
    public async Task GivenInvalidData_WhenUpdatingAnAdditional_ThenItShouldReturnBadRequest()
    {
        // arrange: obtaining the necessary services
        var dbContext = _factory.GetDbContext();

        // arrange: configure authentication
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // arrange: create an existing category
        var category = _fixture.Create<Category>();

        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        // arrange: create an existing additional
        var additional = _fixture.Build<Additional>()
            .With(additional => additional.Category, category)
            .Create();

        await dbContext.Additionals.AddAsync(additional);
        await dbContext.SaveChangesAsync();

        // act: try to update with invalid data (e.g., invalid price)
        var updateRequest = new AdditionalEditingRequest
        {
            AdditionalId = additional.Id,
            Name = "",
            Price = -1,
            CategoryId = category.Id
        };

        var response = await authenticatedClient.PutAsJsonAsync($"/api/additionals/{additional.Id}", updateRequest);

        // assert: verify 400 Bad Request
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Given an existing additional, when deleting, it should return 200 OK and the additional should be removed")]
    public async Task GivenAnExistingAdditional_WhenDeleting_ThenItShouldReturnOkAndRemoveTheAdditional()
    {
        // arrange: configure DbContext and authentication
        var dbContext = _factory.GetDbContext();
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // arrange: create an existing category
        var category = _fixture.Create<Category>();

        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        // arrange: create an existing additional
        var additional = _fixture.Build<Additional>()
            .With(additional => additional.Category, category)
            .Create();

        await dbContext.Additionals.AddAsync(additional);
        await dbContext.SaveChangesAsync();

        // act: delete the additional
        var response = await authenticatedClient.DeleteAsync($"/api/additionals/{additional.Id}");

        // assert: verify successful response (200 OK)
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // act: try to retrieve all additionals and verify the collection is empty
        var getAllResponse = await authenticatedClient.GetFromJsonAsync<Response<IEnumerable<Additional>>>("/api/additionals");

        // assert: verify that the additional was removed
        Assert.NotNull(getAllResponse);
        Assert.NotNull(getAllResponse.Data);

        Assert.Empty(getAllResponse.Data);
        Assert.DoesNotContain(additional, getAllResponse.Data);
    }

    [Fact(DisplayName = "Given a non-existent additional, when deleting, it should return 404 Not Found")]
    public async Task GivenANonExistentAdditional_WhenDeleting_ThenItShouldReturnNotFound()
    {
        // arrange: configure authentication
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: try to delete a non-existent additional
        var response = await authenticatedClient.DeleteAsync("/api/additionals/999");

        // assert: verify 404 Not Found
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    public async Task DisposeAsync() => await Task.CompletedTask;
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
}