namespace Comanda.WebApi.Services;

public interface IAddressService
{
    public Task<Address> GetByZipCodeAsync(string zipCode);
}