namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoryController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        var response = await mediator.Send((CategoryListingRequest) new());
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{categoryId}")]
    public async Task<IActionResult> GetCategoryByIdAsync(int categoryId)
    {
        var response = await mediator.Send(new CategoryDetailRequest
        {
            CategoryId = categoryId
        });

        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateCategoryAsync(CategoryCreationRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{categoryId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> UpdateCategoryAsync(CategoryEditingRequest request, [FromRoute] int categoryId)
    {
        request.CategoryId = categoryId;

        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{categoryId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteCategoryAsync(int categoryId)
    {
        var response = await mediator.Send(new CategoryDeletionRequest
        {
            CategoryId = categoryId
        });

        return StatusCode(response.StatusCode, response);
    }
}