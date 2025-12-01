using System.ComponentModel.DataAnnotations.Schema;

namespace Servexa.Domain.Models
{
    [Table("Categories")]
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }
}
