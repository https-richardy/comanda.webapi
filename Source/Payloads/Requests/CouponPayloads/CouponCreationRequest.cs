namespace Comanda.WebApi.Payloads;

public sealed record CouponCreationRequest : IRequest<Response>
{
    public string Code { get; set; }
    public decimal Discount { get; set; }
    public ECouponType Type { get; set; }
}