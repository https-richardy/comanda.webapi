namespace Comanda.WebApi.Data.Mappings;

public sealed class SettingsEntityConfiguration : IEntityTypeConfiguration<Settings>
{
    private const string _tableName = "Settings";

    public void Configure(EntityTypeBuilder<Settings> builder)
    {
        builder.ToTable(_tableName);
        builder.HasKey(settings => settings.Id);

        builder.Property(settings => settings.AcceptAutomatically)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(settings => settings.MaxConcurrentAutomaticOrders)
            .IsRequired()
            .HasDefaultValue(5);

        builder.Property(settings => settings.EstimatedDeliveryTimeInMinutes)
            .IsRequired()
            .HasDefaultValue(30);

        builder.Property(settings => settings.DeliveryFee)
            .IsRequired()
            .HasDefaultValue(0.0m);

        builder.HasData(new Settings
        {
            Id = 1,
            AcceptAutomatically = false,
            MaxConcurrentAutomaticOrders = 5,
            EstimatedDeliveryTimeInMinutes = 30,
            DeliveryFee = 0.0m
        });
    }
}