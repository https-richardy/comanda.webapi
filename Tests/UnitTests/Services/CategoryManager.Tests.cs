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
}