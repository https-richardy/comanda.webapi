namespace Comanda.TestingSuite.EndToEnd;

[Trait("category", "E2E")]
public sealed class ProductEndpointTests :
    IClassFixture<ApiIntegrationBase<Program, ComandaDbContext>>,
    IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly IFixture _fixture;
    private readonly ApiIntegrationBase<Program, ComandaDbContext> _factory;

    public ProductEndpointTests(ApiIntegrationBase<Program, ComandaDbContext> factory)
    {
        _factory = factory;

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _httpClient = _factory.CreateClient();
    }

    [Fact(DisplayName = "Given a valid request, it must then create a new Product")]
    public async Task GivenAValidRequestItMustThenCreateANewProduct()
    {
        // arrange: obtaining the necessary services
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: creating a category first

        var category = _fixture.Create<Category>();

        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        // arrange: authenticate httpClient as administrator
        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: send a request to create a new product
        var payload = _fixture.Build<ProductCreationRequest>()
            .With(payload => payload.CategoryId, category.Id)
            .Without(payload => payload.Ingredients)
            .Create();

        var response = await authenticatedClient.PostAsJsonAsync("api/products", payload);
        var responseContent = await response.Content.ReadFromJsonAsync<Response<ProductCreationResponse>>();

        response.EnsureSuccessStatusCode();

        // assert: check if the product was created

        Assert.NotNull(responseContent);
        Assert.NotNull(responseContent.Data);

        Assert.True(responseContent.IsSuccess);
        Assert.True(responseContent.Data.ProductId > 0);

        var createdProduct = await dbContext.Products.FirstAsync();

        Assert.Equal(payload.Title, createdProduct.Title);
        Assert.Equal(payload.Description, createdProduct.Description);
        Assert.Equal(payload.CategoryId, createdProduct.Category.Id);
    }

    [Fact(DisplayName = "Given a valid product and image, it must upload the product image")]
    public async Task GivenAValidProductAndImageItMustUploadTheProductImage()
    {
        // arrange: obtain services and create a category
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: create a category
        var category = _fixture.Create<Category>();

        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        // arrange: create a product
        var payload = _fixture.Build<ProductCreationRequest>()
            .With(request => request.CategoryId, category.Id)
            .Without(request => request.Ingredients)
            .Create();

        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // arrange: create product
        var createResponse = await authenticatedClient.PostAsJsonAsync("api/products", payload);
        createResponse.EnsureSuccessStatusCode();

        var createResponseContent = await createResponse.Content.ReadFromJsonAsync<Response<ProductCreationResponse>>();

        Assert.NotNull(createResponseContent);
        Assert.NotNull(createResponseContent.Data);

        var createdProductId = createResponseContent.Data.ProductId;

        // arrange: now simulate uploading an image for the product
        var fileContent = new MultipartFormDataContent
        {
            { new StringContent(createdProductId.ToString()), "ProductId" },
            { new ByteArrayContent(_fixture.Create<byte[]>()), "Image", "image.jpg" }
        };

        // act: upload image
        var uploadImageResponse = await authenticatedClient.PostAsync($"api/products/upload-image/{createdProductId}", fileContent);
        uploadImageResponse.EnsureSuccessStatusCode();

        // assert: verify that the product image path was updated
        var updatedProduct = await dbContext.Products.FirstAsync(product => product.Id == createdProductId);

        Assert.NotNull(updatedProduct.ImagePath);
        Assert.False(string.IsNullOrEmpty(updatedProduct.ImagePath));
    }

    [Fact(DisplayName = "Given a valid product update request, it must update the product")]
    public async Task GivenAValidProductUpdateRequestItMustUpdateTheProduct()
    {
        // arrange: obtain services and create a category
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: create a category
        var category = _fixture.Create<Category>();

        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        // arrange: create a product
        var payload = _fixture.Build<ProductCreationRequest>()
            .With(request => request.CategoryId, category.Id)
            .Without(request => request.Ingredients)
            .Create();

        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: send a request to create a new product
        var createResponse = await authenticatedClient.PostAsJsonAsync("api/products", payload);
        var createResponseContent = await createResponse.Content.ReadFromJsonAsync<Response<ProductCreationResponse>>();

        createResponse.EnsureSuccessStatusCode();

        Assert.NotNull(createResponseContent);
        Assert.NotNull(createResponseContent.Data);

        var createdProductId = createResponseContent.Data.ProductId;

        // arrange: create request to update the product
        var updatePayload = _fixture.Build<ProductEditingRequest>()
            .With(request => request.Title, "Updated Product Title")
            .With(request => request.Description, "Updated Description")
            .With(request => request.CategoryId, category.Id)
            .Create();

        // act: send a request to update the product
        var updateResponse = await authenticatedClient.PutAsJsonAsync($"api/products/{createdProductId}", updatePayload);
        var responseString = await updateResponse.Content.ReadAsStringAsync();

        updateResponse.EnsureSuccessStatusCode();

        // assert: verify if the product was updated
        var updatedProduct = await dbContext.Products.FirstAsync(product => product.Id == createdProductId);

        // verify if the data was updated correctly
        Assert.Equal(updatePayload.Title, updatedProduct.Title);
        Assert.Equal(updatePayload.Description, updatedProduct.Description);
        Assert.Equal(updatePayload.CategoryId, updatedProduct.Category.Id);

        // optional: fetch the product via GET to confirm the updated data
        var getProductResponse = await authenticatedClient.GetAsync($"api/products/{createdProductId}");
        var fetchedProduct = await getProductResponse.Content.ReadFromJsonAsync<Response<FormattedProduct>>();

        getProductResponse.EnsureSuccessStatusCode();

        Assert.NotNull(fetchedProduct);
        Assert.NotNull(fetchedProduct.Data);

        Assert.Equal(updatePayload.Title, fetchedProduct.Data.Title);
        Assert.Equal(updatePayload.Description, fetchedProduct.Data.Description);
    }

    [Fact(DisplayName = "Given a valid product deletion request, it must delete the product")]
    public async Task GivenAValidProductDeletionRequestItMustDeleteTheProduct()
    {
        // arrange: obtain services and create a category
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // arrange: create a category
        var category = _fixture.Create<Category>();

        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        // arrange: create a product
        var payload = _fixture.Build<ProductCreationRequest>()
            .With(request => request.CategoryId, category.Id)
            .Without(request => request.Ingredients)
            .Create();

        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: create product
        var createResponse = await authenticatedClient.PostAsJsonAsync("api/products", payload);
        var createResponseContent = await createResponse.Content.ReadFromJsonAsync<Response<ProductCreationResponse>>();

        createResponse.EnsureSuccessStatusCode();

        Assert.NotNull(createResponseContent);
        Assert.NotNull(createResponseContent.Data);

        var createdProductId = createResponseContent.Data.ProductId;

        // act: send DELETE request to delete the product
        var deleteResponse = await authenticatedClient.DeleteAsync($"api/products/{createdProductId}");
        deleteResponse.EnsureSuccessStatusCode();

        // assert: verify the product is deleted from the database (soft delete)
        var deletedProduct = await dbContext.Products.FindAsync(createdProductId);

        Assert.NotNull(deletedProduct);
        Assert.True(deletedProduct.IsDeleted);

        // assert: verify fetching the product returns 404
        var getProductResponse = await authenticatedClient.GetAsync($"api/products/{createdProductId}");
        var getAllProductsResponse = await authenticatedClient.GetFromJsonAsync<Response<PaginationHelper<FormattedProduct>>>("api/products");

        Assert.Equal(HttpStatusCode.NotFound, getProductResponse.StatusCode);

        Assert.NotNull(getAllProductsResponse);
        Assert.NotNull(getAllProductsResponse.Data);

        Assert.Empty(getAllProductsResponse.Data.Results);
    }

    [Fact(DisplayName = "Given a valid product ID, it must return the product details")]
    public async Task GivenAValidProductIdItMustReturnTheProductDetails()
    {
        // arrange: obtain services and create category
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        var category = _fixture.Create<Category>();

        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        // arrange: create a product
        var payload = _fixture.Build<ProductCreationRequest>()
            .With(request => request.CategoryId, category.Id)
            .Without(request => request.Ingredients)
            .Create();

        var authenticatedClient = await _factory.AuthenticateClientAsync(new AuthenticationCredentials
        {
            Email = "comanda@admin.com",
            Password = "ComandaAdministrator123*"
        });

        // act: create product
        var createResponse = await authenticatedClient.PostAsJsonAsync("api/products", payload);
        var createResponseContent = await createResponse.Content.ReadFromJsonAsync<Response<ProductCreationResponse>>();

        createResponse.EnsureSuccessStatusCode();

        Assert.NotNull(createResponseContent);
        Assert.NotNull(createResponseContent.Data);

        var createdProductId = createResponseContent.Data.ProductId;

        // act: send GET request to retrieve the created product
        var getResponse = await authenticatedClient.GetAsync($"api/products/{createdProductId}");
        getResponse.EnsureSuccessStatusCode();

        // assert: verify if the product was returned correctly
        var getResponseContent = await getResponse.Content.ReadFromJsonAsync<Response<Product>>();

        Assert.NotNull(getResponseContent);
        Assert.NotNull(getResponseContent.Data);

        Assert.True(getResponseContent.IsSuccess);
        Assert.Equal(createdProductId, getResponseContent.Data.Id);
    }

    [Fact(DisplayName = "Given a non-existing product ID, it must return a 404 Not Found")]
    public async Task GivenANonExistingProductIdItMustReturn404NotFound()
    {
        // act: send a GET request to retrieve a non-existing product
        const int nonExistingProductId = 9999;
        var getResponse = await _httpClient.GetAsync($"api/products/{nonExistingProductId}");

        // assert: verify if the response is 404 Not Found
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        // optional: verify if the response message is adequate
        var getResponseContent = await getResponse.Content.ReadFromJsonAsync<Response<Product>>();

        Assert.NotNull(getResponseContent);
        Assert.False(getResponseContent.IsSuccess);
    }

    [Fact(DisplayName = "Given valid request parameters, it must return a list of products")]
    public async Task GivenValidRequestParametersItMustReturnAListOfProducts()
    {
        // arrange: obtain services and create a category
        var services = _factory.GetServiceProvider();
        var dbContext = services.GetRequiredService<ComandaDbContext>();

        // create categories and products
        var category1 = _fixture.Create<Category>();
        var category2 = _fixture.Create<Category>();

        await dbContext.Categories.AddRangeAsync(category1, category2);
        await dbContext.SaveChangesAsync();

        var products = new List<Product>
        {
            _fixture.Build<Product>()
                .With(product => product.Title, "Product 1")
                .With(product => product.Category, category1)
                .Create(),

            _fixture.Build<Product>()
                .With(product => product.Title, "Product 2")
                .With(product => product.Category, category1)
                .Create(),

            _fixture.Build<Product>()
                .With(product => product.Title, "Product 3")
                .With(product => product.Category, category2)
                .Create()
        };

        await dbContext.Products.AddRangeAsync(products);
        await dbContext.SaveChangesAsync();

        // act: send GET request to list the products (with pagination)
        var request = new ProductListingRequest { Page = 1, PageSize = 2 };

        var response = await _httpClient.GetAsync($"api/products?page={request.Page}&pageSize={request.PageSize}");
        var responseContent = await response.Content.ReadFromJsonAsync<Response<PaginationHelper<FormattedProduct>>>();

        response.EnsureSuccessStatusCode();

        // assert: verify if the response contains products and paginated correctly
        Assert.NotNull(responseContent);
        Assert.NotNull(responseContent.Data);

        Assert.NotEmpty(responseContent.Data.Results);
        Assert.Equal(request.PageSize, responseContent.Data.Results.Count()); 
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