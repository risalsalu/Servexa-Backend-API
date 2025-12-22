using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using Servexa.Infrastructure.Repositories.Generic;

namespace Servexa.Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly IDbConnectionFactory _factory;

        public PaymentRepository(IDbConnectionFactory factory) : base(factory)
        {
            _factory = factory;
        }

        private IDbConnection Conn() => _factory.CreateConnection();

        public async Task CreateAsync(Payment payment)
        {
            payment.Id = Guid.NewGuid();
            payment.CreatedOn = DateTime.UtcNow;
            payment.IsDeleted = false;

            const string sql = @"
INSERT INTO Payments
(Id, BookingId, UserId, ShopId, Amount, RazorpayOrderId, RazorpayPaymentId, RazorpaySignature, Status, CreatedOn, IsDeleted)
VALUES
(@Id, @BookingId, @UserId, @ShopId, @Amount, @RazorpayOrderId, @RazorpayPaymentId, @RazorpaySignature, @Status, @CreatedOn, 0)";

            using var conn = Conn();
            await conn.ExecuteAsync(sql, payment);
        }

        public async Task<Payment?> GetByOrderIdAsync(string razorpayOrderId)
        {
            const string sql = @"
SELECT *
FROM Payments
WHERE RazorpayOrderId = @razorpayOrderId AND IsDeleted = 0";

            using var conn = Conn();
            return await conn.QueryFirstOrDefaultAsync<Payment>(sql, new { razorpayOrderId });
        }

        public async Task<bool> UpdateAsync(Payment payment)
        {
            payment.ModifiedOn = DateTime.UtcNow;

            const string sql = @"
UPDATE Payments
SET RazorpayPaymentId = @RazorpayPaymentId,
    RazorpaySignature = @RazorpaySignature,
    Status = @Status,
    ModifiedOn = @ModifiedOn
WHERE Id = @Id AND IsDeleted = 0";

            using var conn = Conn();
            return await conn.ExecuteAsync(sql, payment) > 0;
        }
    }
}
