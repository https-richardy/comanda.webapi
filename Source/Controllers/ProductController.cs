namespace Comanda.WebApi.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProductsAsync([FromQuery] ProductListingRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductByIdAsync(int productId)
    {
        var response = await mediator.Send(new ProductDetailRequest
        {
            ProductId = productId
        });

        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateProductAsync(ProductCreationRequest request)
    {
        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{productId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> UpdateProductAsync(ProductEditingRequest request, [FromRoute] int productId)
    {
        request.ProductId = productId;

        var response = await mediator.Send(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{productId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteProductAsync([FromRoute] int productId)
    {
        var response = await mediator.Send(new ProductDeletionRequest
        {
            ProductId = productId
        });

        return StatusCode(response.StatusCode, response);
    }
}