namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/additionals")]
public sealed class AdditionalController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAdditionalsAsync()
    {
        var response = await mediator.Send((AdditionalsListingRequest) new());
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateAdditionalAsync(AdditionalCreationRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{additionalId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> UpdateAdditionalAsync(AdditionalEditingRequest request, [FromRoute] int additionalId)
    {
        request.AdditionalId = additionalId;

        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{additionalId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteAdditionalAsync([FromRoute] int additionalId)
    {
        var response = await mediator.Send(new AdditionalDeletionRequest
        {
            AdditionalId = additionalId
        });

        return StatusCode(response.StatusCode, response);
    }
}