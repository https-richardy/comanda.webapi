namespace Comanda.WebApi.Handlers;

public sealed class CustomerCurrentOrdersRequest :
    IRequest<Response<IEnumerable<FormattedOrder>>>
{
    
}