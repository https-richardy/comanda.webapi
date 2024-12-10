namespace Comanda.WebApi.Services;

public sealed class AddressManager : IAddressManager
{
    public IQueryable<Address> Addresses => _repository.Entities;

    private readonly IAddressRepository _repository;
    private readonly HttpClient _httpClient;

    public AddressManager(IAddressRepository repository, HttpClient httpClient)
    {
        _repository = repository;
        _httpClient = httpClient;
    }

    public async Task<Address?> GetAddressByIdAsync(int id)
    {
        return await _repository.RetrieveByIdAsync(id);
    }

    public async Task<Address?> FetchAddressByZipCodeAsync(string zipCode)
    {
        zipCode = RemoveNonDigits(zipCode);

        var response = await _httpClient.GetFromJsonAsync<ViaCepResponse>(requestUri: $"/ws/{zipCode}/json/");
        var address = TinyMapper.Map<Address>(response);

        return address;
    }

    public async Task CreateAddressAsync(Address address)
    {
        await _repository.SaveAsync(address);
    }

    public async Task UpdateAddressAsync(Address address)
    {
        await _repository.UpdateAsync(address);
    }

    public async Task DeleteAddressAsync(Address address)
    {
        await _repository.DeleteAsync(address);
    }

    private string RemoveNonDigits(string zipCode)
    {
       return zipCode.Replace("-", "").Trim();
    }
}