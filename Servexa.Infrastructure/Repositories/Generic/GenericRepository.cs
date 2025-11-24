using Dapper;
using System.Data;
using Servexa.Application.Interfaces;
using Servexa.Domain.Models;
using Servexa.Infrastructure.Repositories.Generic;
using Servexa.Domain.Specifications;

namespace Servexa.Infrastructure.Repositories.Generic;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly IDbConnectionFactory _factory;
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

    public async Task<T?> GetByIdAsync(Guid id)
    {
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<T>(
            $"SELECT * FROM {_table} WHERE Id = @id AND IsDeleted = 0",
            new { id });
    }

    public async Task<IEnumerable<T>> FindAsync(ISpecification<T> spec)
    {
        var (where, parameters) = SpecificationEvaluator.GetSql(spec.Criteria);
        using var conn = _factory.CreateConnection();
        return await conn.QueryAsync<T>($"SELECT * FROM {_table} WHERE {where} AND IsDeleted = 0", parameters);
    }

    public async Task<Guid> AddAsync(T entity)
    {
        entity.CreatedOn = DateTime.UtcNow;
        var props = SqlBuilderExtensions.GetProperties<T>();

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
        var props = SqlBuilderExtensions.GetProperties<T>();

        string setClause = string.Join(", ", props.Select(p => $"{p.Name} = @{p.Name}"));
        string sql = $"UPDATE {_table} SET {setClause} WHERE Id = @Id";

        using var conn = _factory.CreateConnection();
        return await conn.ExecuteAsync(sql, entity) > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid deletedBy)
    {
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteAsync(
            $"UPDATE {_table} SET IsDeleted = 1, DeletedBy = @deletedBy, DeletedOn = @now WHERE Id = @id",
            new { id, deletedBy, now = DateTime.UtcNow }) > 0;
    }
}
