namespace Comanda.WebApi.Payloads;

public sealed record CategoryListingRequest :
    IRequest<Response<IEnumerable<FormattedCategory>>>
{

}