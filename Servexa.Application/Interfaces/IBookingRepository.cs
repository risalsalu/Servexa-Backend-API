using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Servexa.Domain.Models;

namespace Servexa.Application.Interfaces
{
    public interface IBookingRepository
    {
        Task<Guid> CreateAsync(Booking booking);
        Task<IEnumerable<Booking>> GetByCustomerAsync(Guid customerId);
    }
}
