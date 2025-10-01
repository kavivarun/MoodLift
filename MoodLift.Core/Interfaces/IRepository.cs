using MoodLift.Core.Entities;
using System.Linq.Expressions;

namespace MoodLift.Core.Interfaces
{
    public interface IRepository<T> where T : AuditableEntity
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(T entity, CancellationToken ct = default);
        Task<List<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
