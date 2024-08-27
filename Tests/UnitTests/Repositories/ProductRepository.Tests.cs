namespace Comanda.TestingSuite.UnitTests.Repositories;


public sealed class ProductRepositoryTests : InMemoryDatabaseFixture<ComandaDbContext>
{
    private readonly IProductRepository _repository;

    public ProductRepositoryTests()
    {
        _repository = new ProductRepository(dbContext: DbContext);

    }

    [Fact(DisplayName = "Given a new product, should save successfully in the database")]
    public async Task GivenNewProduct_ShouldSaveSuccessfullyInTheDatabase()
    {
        var category = Fixture.Create<Category>();
        var product = Fixture.Build<Product>()
            .With(product => product.Category, category)
            .Create();

        await _repository.SaveAsync(product);
        var savedProduct = await DbContext.Products.FindAsync(product.Id);

        Assert.NotNull(savedProduct);
        Assert.Equal(product.Id, savedProduct.Id);
        Assert.Equal(product.Title, savedProduct.Title);
        Assert.Equal(product.Description, savedProduct.Description);
        Assert.Equal(product.Price, savedProduct.Price);
        Assert.Equal(product.Category.Id, savedProduct.Category.Id);
    }

    [Fact(DisplayName = "Given a valid product, should update successfully in the database")]
    public async Task GivenValidProduct_ShouldUpdateSuccessfullyInTheDatabase()
    {
        var category = Fixture.Create<Category>();
        var product = Fixture.Build<Product>()
            .With(product => product.Category, category)
            .Create();

        await DbContext.Products.AddAsync(product);
        await DbContext.SaveChangesAsync();

        product.Title = "Updated Product Title";
        product.Price = 19.99m;

        await _repository.UpdateAsync(product);
        var updatedProduct = await DbContext.Products.FindAsync(product.Id);

        Assert.NotNull(updatedProduct);
        Assert.Equal(product.Id, updatedProduct.Id);
        Assert.Equal(product.Title, updatedProduct.Title);
        Assert.Equal(product.Price, updatedProduct.Price);
    }

    [Fact(DisplayName = "Given a valid product, should delete successfully from the database")]
    public async Task GivenValidProduct_ShouldDeleteSuccessfullyFromTheDatabase()
    {
        var category = Fixture.Create<Category>();
        var product = Fixture.Build<Product>()
            .With(product => product.Category, category)
            .Create();

        await DbContext.Products.AddAsync(product);
        await DbContext.SaveChangesAsync();

        await _repository.DeleteAsync(product);
        var deletedProduct = await DbContext.Products.FindAsync(product.Id);

        Assert.Null(deletedProduct);
    }

    [Fact(DisplayName = "Given a valid predicate, should find all matching products")]
    public async Task GivenValidPredicate_ShouldFindAllMatchingProducts()
    {
        var products = Fixture.CreateMany<Product>(5).ToList();

        products[0].Title = "Special Product";
        products[1].Title = "Special Product";

        await DbContext.Products.AddRangeAsync(products);
        await DbContext.SaveChangesAsync();

        const int pageNumber = 1;
        const int pageSize = 10;

        var foundProducts = await _repository.PagedAsync(
            predicate: product => product.Title.Contains("Special"),
            pageNumber: pageNumber,
            pageSize: pageSize
        );

        Assert.Equal(2, foundProducts.Count());
    }

    [Fact(DisplayName = "Given a valid id, should fetch a product by id")]
    public async Task GivenValidId_ShouldFetchProductById()
    {
        var category = Fixture.Create<Category>();
        var product = Fixture.Build<Product>()
            .With(product => product.Category, category)
            .Create();

        await DbContext.Products.AddAsync(product);
        await DbContext.SaveChangesAsync();

        var foundProduct = await _repository.RetrieveByIdAsync(product.Id);

        Assert.NotNull(foundProduct);
        Assert.Equal(product.Id, foundProduct.Id);
        Assert.Equal(product.Title, foundProduct.Title);
        Assert.Equal(product.Description, foundProduct.Description);
        Assert.Equal(product.Price, foundProduct.Price);
        Assert.Equal(product.Category.Id, foundProduct.Category.Id);
        Assert.Equal(product.Category.Name, foundProduct.Category.Name);
        Assert.Equal(product.ImagePath, foundProduct.ImagePath);
    }

    [Fact(DisplayName = "Should fetch products in pages")]
    public async Task ShouldFetchProductsInPages()
    {
        var products = Fixture.CreateMany<Product>(10).ToList();

        await DbContext.Products.AddRangeAsync(products);
        await DbContext.SaveChangesAsync();

        const int pageNumber = 1;
        const int pageSize = 5;

        var pagedProducts = await _repository.PagedAsync(pageNumber, pageSize);
        Assert.Equal(pageSize, pagedProducts.Count());
    }

    [Fact(DisplayName = "Given a valid id, should return true for existing product")]
    public async Task GivenValidId_ShouldReturnTrueForExistingProduct()
    {
        var category = Fixture.Create<Category>();
        var product = Fixture.Build<Product>()
            .With(product => product.Category, category)
            .Create();

        await DbContext.Products.AddAsync(product);
        await DbContext.SaveChangesAsync();

        var exists = await _repository.ExistsAsync(product.Id);

        Assert.True(exists);
    }

    [Fact(DisplayName = "Given an invalid id, should return false for non-existing product")]
    public async Task GivenInvalidId_ShouldReturnFalseForNonExistingProduct()
    {
        var invalidId = -1;
        var exists = await _repository.ExistsAsync(invalidId);

        Assert.False(exists);
    }

    [Fact(DisplayName = "Should return the correct count of products")]
    public async Task ShouldReturnCorrectCountOfProducts()
    {
        var products = Fixture.CreateMany<Product>(5).ToList();

        await DbContext.Products.AddRangeAsync(products);
        await DbContext.SaveChangesAsync();

        var count = await _repository.CountAsync();

        Assert.Equal(products.Count, count);
    }

    [Fact(DisplayName = "Given a valid predicate, should return the correct count of matching products")]
    public async Task GivenValidPredicate_ShouldReturnCorrectCountOfMatchingProducts()
    {
        var products = Fixture.CreateMany<Product>(5).ToList();

        products[0].Title = "Special Product";
        products[1].Title = "Special Product";

        await DbContext.Products.AddRangeAsync(products);
        await DbContext.SaveChangesAsync();

        var count = await _repository.CountAsync(product => product.Title.Contains("Special"));

        Assert.Equal(2, count);
    }
}