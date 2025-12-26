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
(Id, CustomerId, ShopId, ServiceMode, AddressId, SlotId, TotalAmount, Status, CreatedOn, IsDeleted)
VALUES
(@Id, @CustomerId, @ShopId, @ServiceMode, @AddressId, @SlotId, @TotalAmount, @Status, @CreatedOn, 0)";

            using var conn = Conn();
            await conn.ExecuteAsync(sql, booking);
            return booking.Id;
        }

        public async Task AddItemsAsync(IEnumerable<BookingItem> items)
        {
            const string sql = @"
INSERT INTO BookingServices
(Id, BookingId, ServiceId, Price, DurationInMinutes, CreatedOn, IsDeleted)
VALUES
(@Id, @BookingId, @ServiceId, @Price, @DurationInMinutes, @CreatedOn, 0)";

            using var conn = Conn();

            foreach (var item in items)
            {
                item.Id = Guid.NewGuid();
                item.CreatedOn = DateTime.UtcNow;
                item.IsDeleted = false;
                await conn.ExecuteAsync(sql, item);
            }
        }

        public async Task<IEnumerable<BookingItem>> GetItemsByBookingIdAsync(Guid bookingId)
        {
            const string sql = @"
SELECT *
FROM BookingServices
WHERE BookingId = @bookingId AND IsDeleted = 0";

            using var conn = Conn();
            return await conn.QueryAsync<BookingItem>(sql, new { bookingId });
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

        public async Task<bool> UpdateAsync(Booking booking)
        {
            booking.ModifiedOn = DateTime.UtcNow;

            const string sql = @"
UPDATE Bookings
SET AddressId = @AddressId,
    SlotId = @SlotId,
    Status = @Status,
    ModifiedOn = @ModifiedOn
WHERE Id = @Id AND IsDeleted = 0";

            using var conn = Conn();
            return await conn.ExecuteAsync(sql, booking) > 0;
        }

        public async Task<bool> HasActiveBookingsAsync(Guid customerId)
        {
            const string sql = @"
SELECT COUNT(1)
FROM Bookings
WHERE CustomerId = @customerId
AND IsDeleted = 0
AND Status IN (1,2,3)";

            using var conn = Conn();
            return await conn.ExecuteScalarAsync<int>(sql, new { customerId }) > 0;
        }
    }
}
