namespace Comanda.WebApi.Payloads;

public sealed record CouponEditingRequest : IRequest<Response>
{
    [JsonIgnore] /* this property will be set from the route. */
    public int Id { get; init; }

    public string Code { get; init; }
    public decimal Discount { get; init; }
    public bool IsActive { get; init; }
    public ECouponType Type { get; init; }
    public DateTime ExpirationDate { get; init; }

    public static implicit operator Coupon(CouponEditingRequest payload)
    {
        return new Coupon
        {
            Id = payload.Id,
            Code = payload.Code,
            Discount = payload.Discount,
            IsActive = payload.IsActive,
            Type = payload.Type,
            ExpirationDate = payload.ExpirationDate
        };
    }
}