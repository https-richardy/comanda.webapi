namespace Comanda.TestingSuite.Integration.Services;

public sealed class AddressManagerIntegrationTest : IntegrationFixture<ComandaDbContext>
{
    [Fact(DisplayName = "CreateAddressAsync should save address to the database")]
    public async Task CreateAddressAsync_ShouldSaveAddressToDatabase()
    {
        var addressManager = ServiceProvider.GetRequiredService<IAddressManager>();
        var address = Fixture.Build<Address>()
            .With(address => address.PostalCode, "12345678")
            .With(address => address.Street, "Test Street")
            .With(address => address.Number, "123")
            .With(address => address.City, "Test City")
            .With(address => address.State, "TS")
            .With(address => address.Complement, "Apt 456")
            .Without(address => address.Id)
            .Create();

        await addressManager.CreateAddressAsync(address);
        await DbContext.SaveChangesAsync();

        var savedAddress = await DbContext.Addresses.FirstOrDefaultAsync(address => address.PostalCode == address.PostalCode);

        Assert.NotNull(savedAddress);
        Assert.Equal(address.Street, savedAddress!.Street);
        Assert.Equal(address.Number, savedAddress.Number);
        Assert.Equal(address.City, savedAddress.City);
        Assert.Equal(address.State, savedAddress.State);
        Assert.Equal(address.Complement, savedAddress.Complement);
    }
}