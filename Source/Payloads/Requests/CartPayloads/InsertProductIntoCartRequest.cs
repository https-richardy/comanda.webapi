namespace Comanda.WebApi.Payloads;

public sealed record InsertProductIntoCartRequest :
    AuthenticatedRequest, IRequest<Response>
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}