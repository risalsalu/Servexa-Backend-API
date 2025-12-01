using System;

namespace Servexa.Application.DTOs.Admin
{
    public class AdminShopOwnerListDto
    {
        public Guid Id { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
