using Dapper;
using Servexa.Application.DTOs.Booking;
using Servexa.Application.Interfaces;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Servexa.Infrastructure.Repositories
{
    public class ShopBookingReadRepository : IShopBookingReadRepository
    {
        private readonly IDbConnectionFactory _factory;

        public ShopBookingReadRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        private IDbConnection Conn() => _factory.CreateConnection();

        public async Task<IEnumerable<ShopBookingListItemDto>> GetShopBookingsAsync(Guid shopId)
        {
            const string bookingSql = @"
SELECT 
    b.Id AS BookingId,
    u.FullName AS CustomerName,
    u.Phone AS CustomerPhone,
    b.TotalAmount,
    CASE b.Status
        WHEN 1 THEN 'Draft'
        WHEN 2 THEN 'PendingPayment'
        WHEN 3 THEN 'PaymentFailed'
        WHEN 4 THEN 'Confirmed'
        WHEN 5 THEN 'Cancelled'
        WHEN 6 THEN 'Completed'
    END AS Status,
    b.CreatedOn
FROM Bookings b
INNER JOIN Users u ON u.Id = b.CustomerId
WHERE b.ShopId = @shopId
AND b.IsDeleted = 0
ORDER BY b.CreatedOn DESC";

            const string serviceSql = @"
SELECT 
    bs.BookingId,
    bs.ServiceId,
    bs.Price,
    bs.DurationInMinutes
FROM BookingServices bs
WHERE bs.IsDeleted = 0";

            using var conn = Conn();

            var bookings = (await conn.QueryAsync<ShopBookingListItemDto>(
                bookingSql,
                new { shopId }
            )).ToList();

            if (!bookings.Any())
                return bookings;

            var services = await conn.QueryAsync<dynamic>(serviceSql);

            foreach (var booking in bookings)
            {
                booking.Services = services
                    .Where(s => s.BookingId == booking.BookingId)
                    .Select(s => new BookingItemDto
                    {
                        ServiceId = s.ServiceId,
                        Price = s.Price,
                        DurationInMinutes = s.DurationInMinutes
                    })
                    .ToList();
            }

            return bookings;
        }

        public async Task<bool> UpdateBookingStatusAsync(Guid bookingId, int status)
        {
            const string sql = @"
UPDATE Bookings
SET Status = @status, ModifiedOn = GETUTCDATE()
WHERE Id = @bookingId AND IsDeleted = 0";

            using var conn = Conn();
            return await conn.ExecuteAsync(sql, new { bookingId, status }) > 0;
        }
    }
}
