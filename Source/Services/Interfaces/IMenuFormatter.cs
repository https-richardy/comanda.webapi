namespace Comanda.WebApi.Services;

public interface IMenuFormatter
{
    string Format(IEnumerable<Product> menu);
}