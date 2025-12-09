using System.Collections.Generic;

namespace Servexa.Application.DTOs.Favorites
{
    public class FavoriteServiceListDto
    {
        public IEnumerable<FavoriteServiceDto> Items { get; set; } = new List<FavoriteServiceDto>();
    }
}
