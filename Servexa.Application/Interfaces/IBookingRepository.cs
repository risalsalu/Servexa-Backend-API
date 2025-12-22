using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Servexa.Domain.Models;

namespace Servexa.Application.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task<Guid> CreateAsync(Booking booking);
        Task AddItemsAsync(IEnumerable<BookingItem> items);
        Task<IEnumerable<BookingItem>> GetItemsByBookingIdAsync(Guid bookingId);
        Task<Booking?> GetByIdAsync(Guid bookingId);
        Task<IEnumerable<Booking>> GetByCustomerAsync(Guid customerId);
        Task<IEnumerable<Booking>> GetByShopAsync(Guid shopId);
        Task<bool> UpdateAsync(Booking booking);
    }
}
