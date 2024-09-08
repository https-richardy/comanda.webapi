namespace Comanda.WebApi.Payloads;

public sealed record CouponCreationRequest : IRequest<Response>
{
    public string Code { get; set; }
    public decimal Discount { get; set; }

    public DateTime ExpirationDate { get; set; }
    public ECouponType Type { get; set; }

    public static implicit operator Coupon(CouponCreationRequest request)
    {
        return new Coupon
        {
            Code = request.Code,
            Discount = request.Discount,
            ExpirationDate = request.ExpirationDate,
            Type = request.Type,
            IsActive = true
        };
    }
}