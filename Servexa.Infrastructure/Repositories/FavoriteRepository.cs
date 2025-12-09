using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using Servexa.Infrastructure.Repositories.Generic;

namespace Servexa.Infrastructure.Repositories
{
    public class FavoriteRepository : GenericRepository<UserFavorite>, IFavoriteRepository
    {
        public FavoriteRepository(IDbConnectionFactory factory) : base(factory)
        {
        }
    }
}
