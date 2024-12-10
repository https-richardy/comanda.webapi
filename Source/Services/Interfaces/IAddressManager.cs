namespace Comanda.WebApi.Services;

public interface IAddressManager
{
    public IQueryable<Address> Addresses { get; }

    public Task<Address?> GetAddressByIdAsync(int id);
    public Task<Address?> FetchAddressByZipCodeAsync(string zipCode);

    public Task CreateAddressAsync(Address address);
    public Task UpdateAddressAsync(Address address);
    public Task DeleteAddressAsync(Address address);

    public Task<bool> VerifyAddressExistsAsync(string zipCode);
}
