#pragma warning disable CS8601

namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/establishments")]
public sealed class EstablishmentController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "EstablishmentOwner")]
    public async Task<IActionResult> CreateEstablishmentAsync(CreateEstablishmentRequest request)
    {
        request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("{establishmentId}/products")]
    [Authorize(Roles = "EstablishmentOwner")]
    public async Task<IActionResult> CreateEstablishmentProductAsync(CreateEstablishmentProductRequest request, [FromRoute] int establishmentId)
    {
        request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        request.EstablishmentId = establishmentId;

        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }
}