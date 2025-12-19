using Dapper;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using System.Linq.Expressions;

namespace Servexa.Infrastructure.Repositories.Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly IDbConnectionFactory _factory;
        private readonly string _table;

        public GenericRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
            _table = SqlBuilderExtensions.GetTableName<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<T>($"SELECT * FROM {_table} WHERE IsDeleted = 0");
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            var all = await GetAllAsync();
            return all.AsQueryable().Where(predicate).ToList();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<T>(
                $"SELECT * FROM {_table} WHERE Id = @id AND IsDeleted = 0",
                new { id });
        }

        public async Task<T?> GetOneAsync(Expression<Func<T, bool>> predicate)
        {
            var all = await GetAllAsync();
            return all.AsQueryable().FirstOrDefault(predicate);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            var all = await GetAllAsync();
            return all.AsQueryable().Any(predicate);
        }

        public async Task<Guid> AddAsync(T entity)
        {
            entity.CreatedOn = DateTime.UtcNow;

            var props = SqlBuilderExtensions.GetInsertableProperties<T>();
            string columns = string.Join(", ", props.Select(p => p.Name));
            string values = string.Join(", ", props.Select(p => $"@{p.Name}"));

            string sql = $"INSERT INTO {_table} ({columns}) VALUES ({values})";

            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(sql, entity);

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            entity.ModifiedOn = DateTime.UtcNow;

            var props = SqlBuilderExtensions.GetUpdatableProperties<T>();
            string setClause = string.Join(", ", props.Select(p => $"{p.Name} = @{p.Name}"));

            string sql = $"UPDATE {_table} SET {setClause} WHERE Id = @Id";

            using var conn = _factory.CreateConnection();
            return await conn.ExecuteAsync(sql, entity) > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using var conn = _factory.CreateConnection();
            return await conn.ExecuteAsync(
                $"DELETE FROM {_table} WHERE Id = @id",
                new { id }) > 0;
        }

        public async Task<bool> DeleteSoftAsync(Guid id, Guid deletedBy)
        {
            using var conn = _factory.CreateConnection();
            return await conn.ExecuteAsync(
                $"UPDATE {_table} SET IsDeleted = 1, DeletedBy = @deletedBy, DeletedOn = @now WHERE Id = @id",
                new { id, deletedBy, now = DateTime.UtcNow }) > 0;
        }
    }
}
    