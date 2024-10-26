namespace Comanda.WebApi.Data.Mappings;

public sealed class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
{
    private const string _tableName = "OrderItems";

    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable(_tableName);
        builder.HasKey(orderItem => orderItem.Id);

        builder.Property(orderItem => orderItem.Quantity)
            .IsRequired();

        builder.HasOne(orderItem => orderItem.Product);

        builder.Navigation(orderItem => orderItem.Product)
            .AutoInclude(true);
    }
}