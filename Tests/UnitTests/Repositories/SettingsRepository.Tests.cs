namespace Comanda.TestingSuite.UnitTests.Repositories;

public sealed class SettingsRepositoryTests : SqliteDatabaseFixture<ComandaDbContext>
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

    [Fact(DisplayName = "Given settings ID, should check if it exists in the database")]
    public async Task GivenSettingsId_ShouldCheckIfExistsInDatabase()
    {
        var settings = Fixture.Create<Settings>();

        await DbContext.Settings.AddAsync(settings);
        await DbContext.SaveChangesAsync();

        var exists = await _repository.ExistsAsync(settings.Id);

        Assert.True(exists);
    }

    [Fact(DisplayName = "Given non-existing settings ID, should return false for existence check")]
    public async Task GivenNonExistingSettingsId_ShouldReturnFalseForExistenceCheck()
    {
        var nonExistingId = 999;

        var exists = await _repository.ExistsAsync(nonExistingId);

        Assert.False(exists);
    }

    [Fact(DisplayName = "Given specific criteria, should find single settings matching the criteria")]
    public async Task GivenSpecificCriteria_ShouldFindSingleSettings()
    {
        var settings = Fixture.Create<Settings>();

        await DbContext.Settings.AddAsync(settings);
        await DbContext.SaveChangesAsync();

        var foundSettings = await _repository.FindSingleAsync(s => s.AcceptAutomatically == settings.AcceptAutomatically);

        Assert.NotNull(foundSettings);
        Assert.Equal(settings.AcceptAutomatically, foundSettings.AcceptAutomatically);
    }

    [Fact(DisplayName = "Given specific criteria, should find all settings matching the criteria")]
    public async Task GivenSpecificCriteria_ShouldFindAllSettings()
    {
        var settingsList = Fixture.CreateMany<Settings>(3).ToList();

        await DbContext.Settings.AddRangeAsync(settingsList);
        await DbContext.SaveChangesAsync();

        var foundSettings = await _repository.FindAllAsync(s => s.AcceptAutomatically == settingsList.First().AcceptAutomatically);

        Assert.NotEmpty(foundSettings);
        Assert.All(foundSettings, s => Assert.Equal(settingsList.First().AcceptAutomatically, s.AcceptAutomatically));
    }

    [Fact(DisplayName = "Should retrieve paged collection of settings")]
    public async Task ShouldRetrievePagedSettings()
    {
        var settingsList = Fixture.CreateMany<Settings>(10).ToList();

        await DbContext.Settings.AddRangeAsync(settingsList);
        await DbContext.SaveChangesAsync();

        var pagedSettings = await _repository.PagedAsync(1, 5);

        Assert.Equal(5, pagedSettings.Count());
    }

    [Fact(DisplayName = "Given specific criteria, should retrieve paged collection of settings matching the criteria")]
    public async Task GivenSpecificCriteria_ShouldRetrievePagedSettings()
    {
        var settingsList = Fixture
            .CreateMany<Settings>(5)
            .ToList();

        await DbContext.Settings.AddRangeAsync(settingsList);
        await DbContext.SaveChangesAsync();

        var pagedSettings = await _repository.PagedAsync(settings => settings.AcceptAutomatically == settingsList.First().AcceptAutomatically, 1, 5);

        Assert.All(pagedSettings, settings => Assert.Equal(settingsList.First().AcceptAutomatically, settings.AcceptAutomatically));
    }

    [Fact(DisplayName = "Given a request to count all settings, should return correct count")]
    public async Task GivenRequestToCountAllSettings_ShouldReturnCorrectCount()
    {
        var settingsList = Fixture.CreateMany<Settings>(3).ToList();

        await DbContext.Settings.AddRangeAsync(settingsList);
        await DbContext.SaveChangesAsync();

        var count = await _repository.CountAsync();

        // +1 due to the initial record configured in the Settings entity.
        // This setting ensures that there is always at least one record in the database.
        const int adjustment = 1;

        Assert.Equal(settingsList.Count + adjustment, count);
    }

    [Fact(DisplayName = "Given specific criteria, should return correct count of matching settings")]
    public async Task GivenSpecificCriteria_ShouldReturnCorrectCountOfMatchingSettings()
    {
        var settingsList = Fixture.CreateMany<Settings>(3).ToList();

        await DbContext.Settings.AddRangeAsync(settingsList);
        await DbContext.SaveChangesAsync();

        var count = await _repository.CountAsync(settings => settings.AcceptAutomatically == settingsList.First().AcceptAutomatically);

        Assert.Equal(settingsList.Count(settings => settings.AcceptAutomatically == settingsList.First().AcceptAutomatically), count);
    }

    [Fact(DisplayName = "GetSettingsAsync should retrieve the first settings entry")]
    public async Task GetSettingsAsync_ShouldRetrieveFirstSettingsEntry()
    {
        var retrievedSettings = await _repository.GetSettingsAsync();

        Assert.NotNull(retrievedSettings);
        Assert.Equal(1, retrievedSettings.Id);
    }
}