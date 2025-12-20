using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using Servexa.Infrastructure.Repositories.Generic;

namespace Servexa.Infrastructure.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly IDbConnectionFactory _factory;

        public BookingRepository(IDbConnectionFactory factory) : base(factory)
        {
            _factory = factory;
        }

        private IDbConnection Conn() => _factory.CreateConnection();

        public async Task<Guid> CreateAsync(Booking booking)
        {
            booking.Id = Guid.NewGuid();
            booking.CreatedOn = DateTime.UtcNow;
            booking.IsDeleted = false;

            const string sql = @"
INSERT INTO Bookings
(Id, CustomerId, ShopId, PaymentId, ServiceMode, AddressId, SlotId, Amount, Status, CreatedAt, CreatedOn, IsDeleted)
VALUES
(@Id, @CustomerId, @ShopId, @PaymentId, @ServiceMode, @AddressId, @SlotId, @Amount, @Status, @CreatedAt, @CreatedOn, 0)";

            using var conn = Conn();
            await conn.ExecuteAsync(sql, booking);
            return booking.Id;
        }

        public async Task AddItemsAsync(IEnumerable<BookingItem> items)
        {
            const string sql = @"
INSERT INTO BookingItems
(Id, BookingId, ServiceId, Price, DurationInMinutes, CreatedOn, IsDeleted)
VALUES
(@Id, @BookingId, @ServiceId, @Price, @DurationInMinutes, @CreatedOn, 0)";

            using var conn = Conn();
            foreach (var item in items)
            {
                item.Id = Guid.NewGuid();
                item.CreatedOn = DateTime.UtcNow;
                await conn.ExecuteAsync(sql, item);
            }
        }

        public async Task<IEnumerable<Booking>> GetByCustomerAsync(Guid customerId)
        {
            const string sql = @"
SELECT *
FROM Bookings
WHERE CustomerId = @customerId AND IsDeleted = 0
ORDER BY CreatedOn DESC";

            using var conn = Conn();
            return await conn.QueryAsync<Booking>(sql, new { customerId });
        }

        public async Task<IEnumerable<Booking>> GetByShopAsync(Guid shopId)
        {
            const string sql = @"
SELECT *
FROM Bookings
WHERE ShopId = @shopId AND IsDeleted = 0
ORDER BY CreatedOn DESC";

            using var conn = Conn();
            return await conn.QueryAsync<Booking>(sql, new { shopId });
        }

        public async Task<Booking?> GetByIdAsync(Guid bookingId)
        {
            const string sql = @"
SELECT *
FROM Bookings
WHERE Id = @bookingId AND IsDeleted = 0";

            using var conn = Conn();
            return await conn.QueryFirstOrDefaultAsync<Booking>(sql, new { bookingId });
        }

        public async Task<bool> UpdateStatusAsync(Guid bookingId, BookingStatus status)
        {
            const string sql = @"
UPDATE Bookings
SET Status = @status,
    ModifiedOn = @now
WHERE Id = @bookingId";

            using var conn = Conn();
            return await conn.ExecuteAsync(sql, new
            {
                bookingId,
                status,
                now = DateTime.UtcNow
            }) > 0;
        }
    }
}
