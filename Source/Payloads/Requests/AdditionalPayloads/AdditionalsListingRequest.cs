namespace Comanda.WebApi.Payloads;

public sealed record AdditionalsListingRequest :
    IRequest<Response<IEnumerable<Additional>>>
{

}