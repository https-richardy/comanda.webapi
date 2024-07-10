namespace Comanda.WebApi.Payloads;

public sealed record AddProductToCartRequest : AuthenticatedRequest, IRequest<Response>
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public int EstablishmentId { get; init; }
}