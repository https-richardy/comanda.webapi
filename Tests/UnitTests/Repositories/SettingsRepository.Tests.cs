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
}