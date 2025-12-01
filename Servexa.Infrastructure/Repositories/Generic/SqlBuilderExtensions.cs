using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servexa.Infrastructure.Repositories.Generic
{
    public static class SqlBuilderExtensions
    {
        public static string GetTableName<T>()
        {
            var attr = typeof(T).GetCustomAttribute<TableAttribute>();
            return attr != null ? attr.Name : typeof(T).Name + "s";
        }

        public static IEnumerable<PropertyInfo> GetInsertableProperties<T>()
        {
            return typeof(T).GetProperties();
        }

        public static IEnumerable<PropertyInfo> GetUpdatableProperties<T>()
        {
            return typeof(T).GetProperties().Where(p => p.Name != "Id");
        }
    }
}
