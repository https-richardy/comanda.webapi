namespace Comanda.TestingSuite.UnitTests.Repositories;

public sealed class SettingsRepositoryTests : InMemoryDatabaseFixture<ComandaDbContext>
{
    private readonly ISettingsRepository _repository;

    public SettingsRepositoryTests()
    {
        _repository = new SettingsRepository(dbContext: DbContext);
    }

    [Fact(DisplayName = "Given new settings, should save successfully in the database")]
    public async Task GivenNewSettings_ShouldSaveSuccessfullyInTheDatabase()
    {
        var settings = Fixture.Create<Settings>();

        await _repository.SaveAsync(settings);
        var savedSettings = await DbContext.Settings.FindAsync(settings.Id);

        Assert.NotNull(savedSettings);

        Assert.Equal(settings.Id, savedSettings.Id);
        Assert.Equal(settings.AcceptAutomatically, savedSettings.AcceptAutomatically);
        Assert.Equal(settings.MaxConcurrentAutomaticOrders, savedSettings.MaxConcurrentAutomaticOrders);
        Assert.Equal(settings.EstimatedDeliveryTimeInMinutes, savedSettings.EstimatedDeliveryTimeInMinutes);
        Assert.Equal(settings.DeliveryFee, savedSettings.DeliveryFee);
    }

    [Fact(DisplayName = "Given valid settings, should update successfully in the database")]
    public async Task GivenValidSettings_ShouldUpdateSuccessfullyInTheDatabase()
    {
        var settings = Fixture.Create<Settings>();

        await DbContext.Settings.AddAsync(settings);
        await DbContext.SaveChangesAsync();

        settings.AcceptAutomatically = !settings.AcceptAutomatically;
        settings.MaxConcurrentAutomaticOrders += 1;

        await _repository.UpdateAsync(settings);
        var updatedSettings = await DbContext.Settings.FindAsync(settings.Id);

        Assert.NotNull(updatedSettings);
        Assert.Equal(settings.Id, updatedSettings.Id);
        Assert.Equal(settings.AcceptAutomatically, updatedSettings.AcceptAutomatically);
        Assert.Equal(settings.MaxConcurrentAutomaticOrders, updatedSettings.MaxConcurrentAutomaticOrders);
    }

    [Fact(DisplayName = "Given settings ID, should retrieve correct settings")]
    public async Task GivenSettingsId_ShouldRetrieveCorrectSettings()
    {
        var settings = Fixture.Create<Settings>();

        await DbContext.Settings.AddAsync(settings);
        await DbContext.SaveChangesAsync();

        var retrievedSettings = await _repository.RetrieveByIdAsync(settings.Id);

        Assert.NotNull(retrievedSettings);
        Assert.Equal(settings.Id, retrievedSettings.Id);
        Assert.Equal(settings.AcceptAutomatically, retrievedSettings.AcceptAutomatically);
    }
}