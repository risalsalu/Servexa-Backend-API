using System.Linq.Expressions;
using Dapper;

namespace Servexa.Infrastructure.Repositories.Generic;

public static class SpecificationEvaluator
{
    public static (string sql, DynamicParameters parameters)
        GetSql<T>(Expression<Func<T, bool>>? criteria)
    {
        if (criteria == null)
            return ("1 = 1", new DynamicParameters());

        var binary = (BinaryExpression)criteria.Body;
        string property = binary.Left.ToString().Split('.').Last();
        object value = Expression.Lambda(binary.Right).Compile().DynamicInvoke();

        var parameters = new DynamicParameters();
        parameters.Add($"@{property}", value);

        string sql = $"{property} = @{property}";
        return (sql, parameters);
    }
}
