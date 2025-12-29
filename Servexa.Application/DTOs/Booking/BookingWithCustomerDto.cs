public class BookingWithCustomerDto
{
    public Guid BookingId { get; set; }
    public Guid ShopId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string ServiceMode { get; set; } = null!;
    public Guid? AddressId { get; set; }
    public Guid? SlotId { get; set; }
    public decimal TotalAmount { get; set; }
    public int Status { get; set; }
    public DateTime CreatedOn { get; set; }
}
