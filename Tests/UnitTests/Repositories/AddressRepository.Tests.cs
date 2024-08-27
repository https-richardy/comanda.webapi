namespace Comanda.TestingSuite.UnitTests.Repositories;

public sealed class AddressRepositoryTests : InMemoryDatabaseFixture<ComandaDbContext>
{
    private readonly IAddressRepository _repository;

    public AddressRepositoryTests()
    {
        _repository = new AddressRepository(dbContext: DbContext);
    }

    [Fact(DisplayName = "Given a new address, should save successfully in the database")]
    public async Task GivenNewAddress_ShouldSaveSuccessfullyInTheDatabase()
    {
        var address = Fixture.Create<Address>();

        await _repository.SaveAsync(address);
        var savedAddress = await DbContext.Addresses.FindAsync(address.Id);

        Assert.NotNull(savedAddress);
        Assert.Equal(address.Id, savedAddress.Id);
        Assert.Equal(address.Street, savedAddress.Street);
        Assert.Equal(address.City, savedAddress.City);
    }

    [Fact(DisplayName = "Given a valid address, should update successfully in the database")]
    public async Task GivenValidAddress_ShouldUpdateSuccessfullyInTheDatabase()
    {
        var address = Fixture.Create<Address>();

        await DbContext.Addresses.AddAsync(address);
        await DbContext.SaveChangesAsync();

        address.City = "Updated City";

        await _repository.UpdateAsync(address);
        var updatedAddress = await DbContext.Addresses.FindAsync(address.Id);

        Assert.NotNull(updatedAddress);
        Assert.Equal(address.Id, updatedAddress.Id);
        Assert.Equal(address.City, updatedAddress.City);
    }


    [Fact(DisplayName = "Given a valid address, should delete successfully from the database")]
    public async Task GivenValidAddress_ShouldDeleteSuccessfullyFromTheDatabase()
    {
        var address = Fixture.Create<Address>();

        await DbContext.Addresses.AddAsync(address);
        await DbContext.SaveChangesAsync();

        await _repository.DeleteAsync(address);
        var deletedAddress = await DbContext.Addresses.FindAsync(address.Id);

        Assert.Null(deletedAddress);
    }

    [Fact(DisplayName = "Given a valid predicate, should find all matching addresses")]
    public async Task GivenValidPredicate_ShouldFindAllMatchingAddresses()
    {
        const string cityToSearch = "Rio de Janeiro";
        var addresses = Fixture.CreateMany<Address>(5).ToList();

        addresses[0].City = cityToSearch;
        addresses[1].City = cityToSearch;

        await DbContext.Addresses.AddRangeAsync(addresses);
        await DbContext.SaveChangesAsync();

        var foundAddresses = await _repository.FindAllAsync(address => address.City == cityToSearch);

        Assert.Equal(2, foundAddresses.Count());

        foreach (var address in foundAddresses)
        {
            Assert.Equal(cityToSearch, address.City);
        }
    }
}