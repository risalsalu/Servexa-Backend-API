using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servexa.Domain.Models
{
    [Table("UserFavorites")]
    public class UserFavorite : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ShopServiceId { get; set; }
    }
}
