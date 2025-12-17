using System;
using System.Collections.Generic;

namespace Servexa.Application.DTOs.UserServices
{
    public class UserShopWithServicesDto
    {
        public Guid ShopId { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? OfflineReason { get; set; }
        public List<UserServiceListDto> Services { get; set; } = new();
    }
}
