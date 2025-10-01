using Microsoft.EntityFrameworkCore;
using MoodLift.Core.Interfaces;
using MoodLift.Core.Entities;
using System.Linq.Expressions;

namespace MoodLift.Infrastructure.Repositories
{

    public class Repository<T> : IRepository<T> where T : AuditableEntity
    {
        private readonly MoodLiftDbContext _ctx;
        private readonly DbSet<T> _set;
        public Repository(MoodLiftDbContext ctx) { _ctx = ctx; _set = _ctx.Set<T>(); }

        public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _set.FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task AddAsync(T entity, CancellationToken ct = default)
            => _set.AddAsync(entity, ct).AsTask();

        public Task<List<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => _set.Where(predicate).ToListAsync(ct);

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _ctx.SaveChangesAsync(ct);
    }

}
