namespace Comanda.WebApi.Payloads;

public sealed record GetCartDetailsRequest :
    AuthenticatedRequest, IRequest<Response<CartResponse>>
{
    /* this record inherits 'UserId' from AuthenticatedRequest */
}
