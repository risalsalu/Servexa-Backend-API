using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;

namespace Servexa.Infrastructure.Repositories
{
    public class SlotRepository : ISlotRepository
    {
        private readonly IDbConnectionFactory _factory;

        public SlotRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        private IDbConnection Conn() => _factory.CreateConnection();

        public async Task<bool> HasOverlapAsync(Guid shopId, DateTime start, DateTime end)
        {
            const string sql = @"
SELECT COUNT(1)
FROM Slots
WHERE ShopId = @shopId
AND IsDeleted = 0
AND StartTime < @end
AND EndTime > @start";

            using var db = Conn();
            return await db.ExecuteScalarAsync<int>(sql, new { shopId, start, end }) > 0;
        }

        public async Task<bool> IsSlotAvailableAsync(Guid slotId)
        {
            const string sql = @"
SELECT COUNT(1)
FROM Slots
WHERE Id = @slotId
AND IsBooked = 0
AND IsDeleted = 0";

            using var db = Conn();
            return await db.ExecuteScalarAsync<int>(sql, new { slotId }) > 0;
        }

        public async Task<bool> LockSlotAsync(Guid slotId, Guid customerId)
        {
            const string sql = @"
UPDATE Slots
SET IsBooked = 1,
    BookedBy = @customerId
WHERE Id = @slotId
AND IsBooked = 0
AND IsDeleted = 0";

            using var db = Conn();
            return await db.ExecuteAsync(sql, new { slotId, customerId }) > 0;
        }

        public async Task<bool> MarkBookedAsync(Guid slotId, Guid customerId)
        {
            return await LockSlotAsync(slotId, customerId);
        }

        public async Task<bool> ReleaseAsync(Guid slotId)
        {
            const string sql = @"
UPDATE Slots
SET IsBooked = 0,
    BookedBy = NULL
WHERE Id = @slotId
AND IsDeleted = 0";

            using var db = Conn();
            return await db.ExecuteAsync(sql, new { slotId }) > 0;
        }

        public async Task<bool> SlotExistsAsync(Guid shopId, DateTime start, DateTime end)
        {
            const string sql = @"
SELECT COUNT(1)
FROM Slots
WHERE ShopId = @shopId
AND IsDeleted = 0
AND StartTime < @end
AND EndTime > @start";

            using var db = Conn();
            return await db.ExecuteScalarAsync<int>(sql, new { shopId, start, end }) > 0;
        }

        public async Task<bool> SlotsExistForDateAsync(Guid shopId, DateTime date)
        {
            const string sql = @"
SELECT COUNT(1)
FROM Slots
WHERE ShopId = @shopId
AND CAST(StartTime AS DATE) = @date
AND IsDeleted = 0";

            using var db = Conn();
            return await db.ExecuteScalarAsync<int>(sql, new { shopId, date = date.Date }) > 0;
        }

        public async Task AddAsync(Slot slot)
        {
            slot.Id = Guid.NewGuid();
            slot.CreatedOn = DateTime.UtcNow;
            slot.IsDeleted = false;

            const string sql = @"
INSERT INTO Slots
(Id, ShopId, StartTime, EndTime, IsBooked, BookedBy, IsDeleted, CreatedOn)
VALUES
(@Id, @ShopId, @StartTime, @EndTime, 0, NULL, 0, @CreatedOn)";

            using var db = Conn();
            await db.ExecuteAsync(sql, slot);
        }

        public async Task<IEnumerable<Slot>> GetAvailableSlotsAsync(Guid shopId, DateTime date)
        {
            const string sql = @"
SELECT *
FROM Slots
WHERE ShopId = @shopId
AND CAST(StartTime AS DATE) = @date
AND IsBooked = 0
AND IsDeleted = 0
ORDER BY StartTime";

            using var db = Conn();
            return await db.QueryAsync<Slot>(sql, new
            {
                shopId,
                date = date.Date
            });
        }

        public async Task<Slot?> GetByIdAsync(Guid slotId)
        {
            const string sql = @"
SELECT *
FROM Slots
WHERE Id = @slotId
AND IsDeleted = 0";

            using var db = Conn();
            return await db.QueryFirstOrDefaultAsync<Slot>(sql, new { slotId });
        }

        public async Task<bool> DeleteAsync(Guid slotId)
        {
            const string sql = @"
UPDATE Slots
SET IsDeleted = 1
WHERE Id = @slotId
AND IsDeleted = 0";

            using var db = Conn();
            return await db.ExecuteAsync(sql, new { slotId }) > 0;
        }
    }
}
