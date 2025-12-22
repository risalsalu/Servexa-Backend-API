namespace Servexa.Domain.Models
{
    public enum BookingStatus
    {
        Draft = 1,
        PendingPayment = 2,
        PaymentFailed = 3,
        Confirmed = 4,
        Cancelled = 5,
        Completed = 6
    }
}
