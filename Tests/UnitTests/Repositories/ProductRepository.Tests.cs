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
}