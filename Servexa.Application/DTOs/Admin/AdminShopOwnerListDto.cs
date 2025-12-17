using System;

namespace Servexa.Application.DTOs.Admin
{
    public class AdminShopOwnerListDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public Guid ShopId { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool ShopIsActive { get; set; }
        public string? ShopOfflineReason { get; set; }
    }
}
