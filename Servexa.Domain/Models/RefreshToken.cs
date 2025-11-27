using System;

namespace Servexa.Domain.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsDeleted { get; set; }
    }
}
