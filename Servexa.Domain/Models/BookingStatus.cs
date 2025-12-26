namespace Servexa.Domain.Models
{
    public enum BookingStatus
    {
        Draft = 1,
        PendingPayment = 2,
        Confirmed = 3,
        Completed = 4,
        Cancelled = 5,
        PaymentFailed = 6
    }
}
