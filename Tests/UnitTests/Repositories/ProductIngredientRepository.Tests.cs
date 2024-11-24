namespace Comanda.TestingSuite.UnitTests.Repositories;

public sealed class ProductIngredientRepositoryTests : SqliteDatabaseFixture<ComandaDbContext>
{
    private readonly IProductIngredientRepository _repository;

    public ProductIngredientRepositoryTests()
    {
        _repository = new ProductIngredientRepository(dbContext: DbContext);
    }

    [Fact(DisplayName = "Given new product ingredient, should save successfully in the database")]
    public async Task GivenNewProductIngredient_ShouldSaveSuccessfullyInTheDatabase()
    {
        var productIngredient = Fixture.Create<ProductIngredient>();
        await _repository.SaveAsync(productIngredient);

        var savedProductIngredient = await DbContext.ProductIngredients
            .Include(productIngredient => productIngredient.Ingredient)
            .FirstOrDefaultAsync(productIngredient => productIngredient.Id == productIngredient.Id);

        Assert.NotNull(savedProductIngredient);

        Assert.Equal(productIngredient.Id, savedProductIngredient.Id);
        Assert.Equal(productIngredient.StandardQuantity, savedProductIngredient.StandardQuantity);
        Assert.Equal(productIngredient.IsMandatory, savedProductIngredient.IsMandatory);
        Assert.Equal(productIngredient.Ingredient, savedProductIngredient.Ingredient);
    }

    [Fact(DisplayName = "Given valid product ingredient, should update successfully in the database")]
    public async Task GivenValidProductIngredient_ShouldUpdateSuccessfullyInTheDatabase()
    {
        var productIngredient = Fixture.Create<ProductIngredient>();

        await DbContext.ProductIngredients.AddAsync(productIngredient);
        await DbContext.SaveChangesAsync();

        productIngredient.StandardQuantity = 10;
        productIngredient.IsMandatory = false;

        await _repository.UpdateAsync(productIngredient);

        var updatedProductIngredient = await DbContext.ProductIngredients
            .Include(productIngredient => productIngredient.Ingredient)
            .FirstOrDefaultAsync(productIngredient => productIngredient.Id == productIngredient.Id);

        Assert.NotNull(updatedProductIngredient);
        Assert.Equal(productIngredient.StandardQuantity, updatedProductIngredient.StandardQuantity);
        Assert.Equal(productIngredient.IsMandatory, updatedProductIngredient.IsMandatory);
    }

    [Fact(DisplayName = "Given product ingredient ID, should retrieve correct product ingredient")]
    public async Task GivenProductIngredientId_ShouldRetrieveCorrectProductIngredient()
    {
        var productIngredient = Fixture.Create<ProductIngredient>();

        await DbContext.ProductIngredients.AddAsync(productIngredient);
        await DbContext.SaveChangesAsync();

        var retrievedProductIngredient = await _repository.RetrieveByIdAsync(productIngredient.Id);

        Assert.NotNull(retrievedProductIngredient);
        Assert.Equal(productIngredient.Id, retrievedProductIngredient.Id);
        Assert.Equal(productIngredient.StandardQuantity, retrievedProductIngredient.StandardQuantity);
        Assert.Equal(productIngredient.IsMandatory, retrievedProductIngredient.IsMandatory);
    }

    [Fact(DisplayName = "Given valid product ingredient, should delete successfully from the database")]
    public async Task GivenValidProductIngredient_ShouldDeleteSuccessfullyFromDatabase()
    {
        var productIngredient = Fixture.Create<ProductIngredient>();

        await DbContext.ProductIngredients.AddAsync(productIngredient);
        await DbContext.SaveChangesAsync();

        await _repository.DeleteAsync(productIngredient);
        var deletedProductIngredient = await DbContext.ProductIngredients.FindAsync(productIngredient.Id);

        Assert.NotNull(deletedProductIngredient);
        Assert.True(deletedProductIngredient.IsDeleted);
    }

    [Fact(DisplayName = "Should retrieve all product ingredients")]
    public async Task ShouldRetrieveAllProductIngredients()
    {
        var productIngredients = Fixture.CreateMany<ProductIngredient>(10).ToList();

        await DbContext.ProductIngredients.AddRangeAsync(productIngredients);
        await DbContext.SaveChangesAsync();

        var retrievedProductIngredients = await _repository.RetrieveAllAsync();

        Assert.Equal(productIngredients.Count, retrievedProductIngredients.Count());
        foreach (var productIngredient in productIngredients)
        {
            Assert.Contains(retrievedProductIngredients, productIngredient => productIngredient.Id == productIngredient.Id);
        }
    }
}