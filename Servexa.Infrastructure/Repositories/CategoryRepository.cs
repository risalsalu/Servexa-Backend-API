using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using Servexa.Infrastructure.Repositories.Generic;

namespace Servexa.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly IDbConnectionFactory _factory;

        public CategoryRepository(IDbConnectionFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            using var conn = _factory.CreateConnection();
            var sql = @"SELECT COUNT(1) 
                        FROM Categories 
                        WHERE LOWER(Name) = LOWER(@name) AND IsDeleted = 0";
            return await conn.ExecuteScalarAsync<int>(sql, new { name }) > 0;
        }

        public async Task<bool> ExistsByNameExceptIdAsync(Guid id, string name)
        {
            using var conn = _factory.CreateConnection();
            var sql = @"SELECT COUNT(1) 
                        FROM Categories 
                        WHERE LOWER(Name) = LOWER(@name) 
                        AND Id <> @id 
                        AND IsDeleted = 0";
            return await conn.ExecuteScalarAsync<int>(sql, new { id, name }) > 0;
        }
    }
}
