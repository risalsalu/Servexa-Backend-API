using System.Reflection;

namespace Servexa.Infrastructure.Repositories.Generic
{
    public static class SqlBuilderExtensions
    {
        public static string GetTableName<T>()
            => typeof(T).Name + "s";

        public static IEnumerable<PropertyInfo> GetProperties<T>()
        {
            return typeof(T).GetProperties().Where(p => p.Name != "Id");
        }
    }
}
