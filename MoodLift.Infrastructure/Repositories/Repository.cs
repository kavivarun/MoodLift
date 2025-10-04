using Microsoft.EntityFrameworkCore;
using MoodLift.Core.Interfaces;
using MoodLift.Core.Entities;
using System.Linq.Expressions;

namespace MoodLift.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository implementation for Entity Framework Core that works with auditable entities.
    /// Provides basic CRUD operations for entities that inherit from AuditableEntity.
    /// </summary>
    /// <typeparam name="T">The type of entity, must inherit from AuditableEntity.</typeparam>
    public class Repository<T> : IRepository<T> where T : AuditableEntity
    {
        private readonly MoodLiftDbContext _ctx;
        private readonly DbSet<T> _set;

        /// <summary>
        /// Initializes a new instance of the Repository class.
        /// </summary>
        /// <param name="ctx">The database context used to perform operations.</param>
        public Repository(MoodLiftDbContext ctx) 
        { 
            _ctx = ctx; 
            _set = _ctx.Set<T>(); 
        }

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <param name="ct">Optional cancellation token to cancel the operation.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _set.FirstOrDefaultAsync(x => x.Id == id, ct);

        /// <summary>
        /// Adds a new entity to the database context.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="ct">Optional cancellation token to cancel the operation.</param>
        public Task AddAsync(T entity, CancellationToken ct = default)
            => _set.AddAsync(entity, ct).AsTask();

        /// <summary>
        /// Retrieves a list of entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">An expression to filter the entities.</param>
        /// <param name="ct">Optional cancellation token to cancel the operation.</param>
        /// <returns>A list of entities matching the predicate.</returns>
        public Task<List<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => _set.Where(predicate).ToListAsync(ct);

        /// <summary>
        /// Persists all changes made to the database context to the database.
        /// </summary>
        /// <param name="ct">Optional cancellation token to cancel the operation.</param>
        /// <returns>The number of state entries written to the database.</returns>
        /// <remarks>
        /// This method must be called to persist any changes made through AddAsync.
        /// The context's SaveChanges will automatically handle auditing properties.
        /// </remarks>
        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _ctx.SaveChangesAsync(ct);
    }
}
