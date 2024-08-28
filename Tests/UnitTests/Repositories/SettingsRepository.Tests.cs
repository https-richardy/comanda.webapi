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

    [Fact(DisplayName = "Given valid settings, should delete successfully from the database")]
    public async Task GivenValidSettings_ShouldDeleteSuccessfullyFromDatabase()
    {
        var settings = Fixture.Create<Settings>();

        await DbContext.Settings.AddAsync(settings);
        await DbContext.SaveChangesAsync();

        await _repository.DeleteAsync(settings);
        var deletedSettings = await DbContext.Settings.FindAsync(settings.Id);

        Assert.Null(deletedSettings);
    }

    [Fact(DisplayName = "Should retrieve all settings")]
    public async Task ShouldRetrieveAllSettings()
    {
        var settingsList = Fixture.CreateMany<Settings>(3).ToList();

        await DbContext.Settings.AddRangeAsync(settingsList);
        await DbContext.SaveChangesAsync();

        var retrievedSettings = await _repository.RetrieveAllAsync();

        // +1 due to the initial record configured in the Settings entity.
        // This setting ensures that there is always at least one record in the database.
        const int adjustment = 1;

        Assert.Equal(settingsList.Count + adjustment, retrievedSettings.Count());
        foreach (var settings in settingsList)
        {
            Assert.Contains(retrievedSettings, settings => settings.Id == settings.Id);
        }
    }
}