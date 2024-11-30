namespace Comanda.WebApi.Payloads;

public sealed record FetchCustomerAddressesRequest :
    IRequest<Response<IEnumerable<FormattedAddress>>>
{
    
}