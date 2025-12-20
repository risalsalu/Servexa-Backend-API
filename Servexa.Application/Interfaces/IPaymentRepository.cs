using System.Threading.Tasks;
using Servexa.Domain.Models;

namespace Servexa.Application.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task CreateAsync(Payment payment);
        Task<Payment?> GetByOrderIdAsync(string razorpayOrderId);
        Task<bool> UpdateAsync(Payment payment);
    }
}
