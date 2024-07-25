namespace Comanda.WebApi.Data.Mappings;

public sealed class OpeningHoursEntityConfiguration : IEntityTypeConfiguration<OpeningHours>
{
    private const string _tableName = "OpeningHours";

    public void Configure(EntityTypeBuilder<OpeningHours> builder)
    {
        builder.ToTable(_tableName);
        builder.HasKey(openingHours => openingHours.Id);

        builder.Property(openingHours => openingHours.DayOfWeek)
            .IsRequired();

        builder.Property(openingHours => openingHours.StartTime)
            .IsRequired();

        builder.Property(openingHours => openingHours.EndTime)
            .IsRequired();
    }
}