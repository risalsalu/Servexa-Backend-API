using System;
using System.Threading.Tasks;
using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using Servexa.Infrastructure.Repositories.Generic;

namespace Servexa.Infrastructure.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(IDbConnectionFactory factory) : base(factory) { }

        public async Task UpdateStatusAsync(Guid bookingId, BookingStatus status, Guid updatedBy)
        {
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(
                "UPDATE Bookings SET Status=@status, ModifiedBy=@updatedBy, ModifiedOn=SYSUTCDATETIME() WHERE Id=@bookingId AND IsDeleted=0",
                new { status, updatedBy, bookingId });
        }
    }
}
