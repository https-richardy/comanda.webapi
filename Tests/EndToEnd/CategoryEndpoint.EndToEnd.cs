namespace Comanda.TestingSuite.EndToEnd;

public sealed class CategoryEndpointTests : WebApiFixture<ComandaDbContext>
{
    public CategoryEndpointTests(WebApiFactoryFixture<Program> factory)
        : base(factory) {  }

    [Fact(DisplayName = "Given a valid request, it must then create a new category")]
    public async Task GivenAValidRequestItMustThenCreateANewCategory()
    {
        await AuthenticateAdminUserAsync();
        var client = GetAuthenticatedClient();

        var payload = new CategoryCreationRequest
        {
            Title = "Food"
        };

        var response = await client.PostAsJsonAsync("api/categories", payload);
        var responseContent = await response.Content.ReadFromJsonAsync<Response>();

        response.EnsureSuccessStatusCode();

        Assert.NotNull(responseContent);
        Assert.True(responseContent.IsSuccess);
    }
}