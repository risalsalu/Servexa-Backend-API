using System;
using System.Collections.Generic;
using Servexa.Domain.Models;

namespace Servexa.Application.DTOs.Booking
{
    public class CreateBookingDto
    {
        public Guid ShopId { get; set; }
        public ServiceMode ServiceMode { get; set; }
        public IEnumerable<Guid> ServiceIds { get; set; } = [];
    }
}
