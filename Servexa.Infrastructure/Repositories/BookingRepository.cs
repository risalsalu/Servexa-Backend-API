using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IDbConnectionFactory _factory;

        public BookingRepository(IDbConnectionFactory factory)
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
(Id, CustomerId, ShopId, ServiceMode, AddressId, SlotId, Amount, Status, IsDeleted, CreatedOn)
VALUES
(@Id, @CustomerId, @ShopId, @ServiceMode, @AddressId, @SlotId, @Amount, @Status, 0, @CreatedOn)";

            using var db = Conn();
            await db.ExecuteAsync(sql, booking);
            return booking.Id;
        }

        public async Task<IEnumerable<Booking>> GetByCustomerAsync(Guid customerId)
        {
            const string sql = @"
SELECT * FROM Bookings
WHERE CustomerId = @customerId AND IsDeleted = 0
ORDER BY CreatedOn DESC";

            using var db = Conn();
            return await db.QueryAsync<Booking>(sql, new { customerId });
        }
    }
}
