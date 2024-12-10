namespace Comanda.TestingSuite.UnitTests.Services;

public sealed class CategoryManagerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository;
    private readonly ICategoryManager _categoryManager;
    private readonly IFixture _fixture;

    public CategoryManagerTests()
    {
        _categoryRepository = new Mock<ICategoryRepository>();

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _categoryManager = new CategoryManager(_categoryRepository.Object);
    }

    [Fact(DisplayName = "GetAsync should return category when id exists")]
    public async Task GetAsyncShouldReturnCategoryWhenIdExists()
    {
        /* arrange: set up a valid category id and a category with the given id */
        var categoryId = _fixture.Create<int>();
        var category = _fixture.Build<Category>()
            .With(category => category.Id, categoryId)
            .Create();

        /* arrange: set up the repository to return the category when asked for the given id */
        _categoryRepository
            .Setup(repository => repository.RetrieveByIdAsync(categoryId))
            .ReturnsAsync(category);

        /* act: call the get method with the given id */
        var result = await _categoryManager.GetAsync(categoryId);

        /* assert: verify that the returned category is the same as the one set up */
        Assert.NotNull(result);
        Assert.Equal(category.Id, result.Id);
    }

    [Fact(DisplayName = "GetAsync should return null when id does not exist")]
    public async Task GetAsyncShouldReturnNullWhenIdDoesNotExist()
    {
        /* arrange: setup a valid category id but no category with this id */
        var categoryId = _fixture.Create<int>();

        /* arrange: setup the repository to return null when asked for the given id */
        _categoryRepository
            .Setup(repository => repository.RetrieveByIdAsync(categoryId))
            .ReturnsAsync((Category?)null!);

        /* act: call the get method with the given id */
        var result = await _categoryManager.GetAsync(categoryId);

        /* assert: verify that the returned category is null */
        Assert.Null(result);
    }

    [Fact(DisplayName = "GetAllAsync should return all categories")]
    public async Task GetAllAsyncShouldReturnAllCategories()
    {
        /* arrange: set up a list of categories */
        var categories = _fixture
            .CreateMany<Category>()
            .ToList();

        /* arrange: configure the repository to return the list of categories */
        _categoryRepository
            .Setup(repository => repository.RetrieveAllAsync())
            .ReturnsAsync(categories);

        /* act: retrieve all categories using the category manager */
        var result = await _categoryManager.GetAllAsync();

        /* assert: verify that the result is not null and matches the expected count */
        Assert.NotNull(result);
        Assert.Equal(categories.Count, result.Count());
    }

    [Fact(DisplayName = "CreateAsync should save new category")]
    public async Task CreateAsyncShouldSaveNewCategory()
    {
        /* arrange: set up a new category */
        var category = _fixture.Create<Category>();

        /* act: call the create method with the new category */
        await _categoryManager.CreateAsync(category);

        /* assert: verify that the repository was called once with the new category */
        _categoryRepository.Verify(repository => repository.SaveAsync(category), Times.Once);
    }

    [Fact(DisplayName = "UpdateAsync should update existing category")]
    public async Task UpdateAsyncShouldUpdateExistingCategory()
    {
        /* arrange: create a category that will be updated */
        var category = _fixture.Create<Category>();

        /* act: call the update method with the category */
        await _categoryManager.UpdateAsync(category);

        /* assert: verify that the repository was called once with the updated category */
        _categoryRepository.Verify(repository => repository.UpdateAsync(category), Times.Once);
    }

    [Fact(DisplayName = "DeleteAsync should delete category")]
    public async Task DeleteAsyncShouldDeleteCategory()
    {
        /* Arrange: setup a category that will be deleted */
        var category = _fixture.Create<Category>();

        /* Act: call the delete method with the category */
        await _categoryManager.DeleteAsync(category);

        /* Assert: verify that the repository was called once with the deleted category */
        _categoryRepository.Verify(repository => repository.DeleteAsync(category), Times.Once);
    }

    [Fact(DisplayName = "Categories property should return IQueryable")]
    public void CategoriesShouldReturnIQueryable()
    {
        /* arrange: set up a list of categories */
        var categories = _fixture
            .CreateMany<Category>()
            .AsQueryable();

        /* arrange: configure the repository.Entities to return the list of categories (as queryable) */
        _categoryRepository
            .Setup(repository => repository.Entities)
            .Returns(categories);

        /* act: call the Categories property */
        var result = _categoryManager.Categories;

        /* assert: verify that the result is not null */
        Assert.NotNull(result);

        /* assert: verify that the result is equal to the list of categories */
        Assert.Equal(categories, result);
    }
}