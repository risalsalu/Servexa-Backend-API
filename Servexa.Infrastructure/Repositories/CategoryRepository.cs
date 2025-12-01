using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using Servexa.Infrastructure.Repositories.Generic;

namespace Servexa.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(IDbConnectionFactory factory) : base(factory)
        {
        }
    }
}
