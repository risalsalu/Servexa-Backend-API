using Servexa.Domain.Models;

namespace Servexa.Application.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsByNameExceptIdAsync(Guid id, string name);
    }
}
