namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/ingredients")]
public sealed class IngredientController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllIngredientsAsync()
    {
        var response = await mediator.Send((IngredientListingRequest) new());
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateIngredientAsync(IngredientCreationRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{ingredientId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> UpdateIngredientAsync(IngredientEditingRequest request, [FromRoute] int ingredientId)
    {
        request.IngredientId = ingredientId;

        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{ingredientId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteIngredientAsync(int ingredientId)
    {
        var response = await mediator.Send(new IngredientDeletionRequest
        {
            IngredientId = ingredientId
        });

        return StatusCode(response.StatusCode, response);
    }
}