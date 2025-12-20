using System;
using System.Collections.Generic;

namespace Servexa.Application.DTOs.Payment
{
    public class CreatePaymentOrderDto
    {
        public Guid ShopId { get; set; }
        public string ServiceMode { get; set; } = null!;
        public Guid? AddressId { get; set; }
        public Guid? SlotId { get; set; }
        public IEnumerable<CreatePaymentServiceDto> Services { get; set; } = [];
        public decimal Amount { get; set; }
    }
}
