namespace Comanda.TestingSuite.UnitTests.Repositories;

public sealed class IngredientRepositoryTests : SqliteDatabaseFixture<ComandaDbContext>
{
    private readonly IIngredientRepository _repository;

    public IngredientRepositoryTests()
    {
        _repository = new IngredientRepository(dbContext: DbContext);
    }

    [Fact(DisplayName = "Given a new ingredient, should save successfully in the database")]
    public async Task GivenNewIngredient_ShouldSaveSuccessfullyInTheDatabase()
    {
        var ingredient = Fixture.Create<Ingredient>();

        await _repository.SaveAsync(ingredient);
        var savedIngredient = await DbContext.Ingredients.FindAsync(ingredient.Id);

        Assert.NotNull(savedIngredient);
        Assert.Equal(ingredient.Id, savedIngredient.Id);
        Assert.Equal(ingredient.Name, savedIngredient.Name);
    }

    [Fact(DisplayName = "Given a valid ingredient, should update successfully in the database")]
    public async Task GivenValidIngredient_ShouldUpdateSuccessfullyInTheDatabase()
    {
        var ingredient = Fixture.Create<Ingredient>();

        await DbContext.Ingredients.AddAsync(ingredient);
        await DbContext.SaveChangesAsync();

        ingredient.Name = "Updated Name";

        await _repository.UpdateAsync(ingredient);
        var updatedIngredient = await DbContext.Ingredients.FindAsync(ingredient.Id);

        Assert.NotNull(updatedIngredient);
        Assert.Equal(ingredient.Id, updatedIngredient.Id);
        Assert.Equal(ingredient.Name, updatedIngredient.Name);
    }

    [Fact(DisplayName = "Given an ingredient ID, should retrieve the correct ingredient")]
    public async Task GivenIngredientId_ShouldRetrieveCorrectIngredient()
    {
        var ingredient = Fixture.Create<Ingredient>();

        await DbContext.Ingredients.AddAsync(ingredient);
        await DbContext.SaveChangesAsync();

        var retrievedIngredient = await _repository.RetrieveByIdAsync(ingredient.Id);

        Assert.NotNull(retrievedIngredient);
        Assert.Equal(ingredient.Id, retrievedIngredient.Id);
        Assert.Equal(ingredient.Name, retrievedIngredient.Name);
    }

    [Fact(DisplayName = "Given a valid ingredient, should delete successfully from the database")]
    public async Task GivenValidIngredient_ShouldDeleteSuccessfullyFromDatabase()
    {
        var ingredient = Fixture.Create<Ingredient>();

        await DbContext.Ingredients.AddAsync(ingredient);
        await DbContext.SaveChangesAsync();

        await _repository.DeleteAsync(ingredient);
        var deletedIngredient = await DbContext.Ingredients.FindAsync(ingredient.Id);

        Assert.NotNull(deletedIngredient);
        Assert.True(deletedIngredient.IsDeleted);
    }

    [Fact(DisplayName = "Should retrieve all ingredients")]
    public async Task ShouldRetrieveAllIngredients()
    {
        var ingredients = Fixture.CreateMany<Ingredient>(3).ToList();

        await DbContext.Ingredients.AddRangeAsync(ingredients);
        await DbContext.SaveChangesAsync();

        var retrievedIngredients = await _repository.RetrieveAllAsync();

        Assert.Equal(ingredients.Count, retrievedIngredients.Count());
        foreach (var ingredient in ingredients)
        {
            Assert.Contains(retrievedIngredients, ingredient => ingredient.Id == ingredient.Id && ingredient.Name == ingredient.Name);
        }
    }
}