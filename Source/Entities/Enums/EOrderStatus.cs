namespace Comanda.WebApi.Entities;

public enum EOrderStatus
{
    Pending,
    Confirmed,
    InPreparation,
    Shipped,
    Returned,
    Delivered,
    CancelledByCustomer,
    CancelledBySystem
}