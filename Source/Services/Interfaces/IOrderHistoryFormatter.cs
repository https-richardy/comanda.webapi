namespace Comanda.WebApi.Services;

public interface IOrderHistoryFormatter
{
    string Format(IEnumerable<Order> orderHistory);
}