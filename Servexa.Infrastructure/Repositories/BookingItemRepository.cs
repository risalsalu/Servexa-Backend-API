using System;
using System.Threading.Tasks;
using Dapper;
using Servexa.Application.DTOs.Booking;
using Servexa.Application.Interfaces;

namespace Servexa.Infrastructure.Repositories
{
    public class BookingItemRepository : IBookingItemRepository
    {
        private readonly IDbConnectionFactory _factory;

        public BookingItemRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task AddAsync(Guid bookingId, BookingItemDto dto, Guid createdBy)
        {
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(
                @"INSERT INTO BookingItems
                  (Id,BookingId,ServiceId,ServiceName,Price,DurationMinutes,CreatedBy,CreatedOn)
                  VALUES
                  (NEWID(),@BookingId,@ServiceId,@ServiceName,@Price,@DurationMinutes,@CreatedBy,SYSUTCDATETIME())",
                new
                {
                    BookingId = bookingId,
                    dto.ServiceId,
                    dto.ServiceName,
                    dto.Price,
                    dto.DurationMinutes,
                    CreatedBy = createdBy
                });
        }
    }
}
