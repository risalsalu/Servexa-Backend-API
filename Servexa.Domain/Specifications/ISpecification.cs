using System.Linq.Expressions;

namespace Servexa.Domain.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
}
