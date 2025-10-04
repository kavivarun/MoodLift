using Microsoft.EntityFrameworkCore;
using MoodLift.Core.Entities;

namespace MoodLift.Infrastructure.Repositories
{
    /// <summary>
    /// Database context for the MoodLift application.
    /// Handles entity configuration and automatic auditing of entities.
    /// </summary>
    public class MoodLiftDbContext : DbContext
    {
        /// <summary>
        /// Gets the DbSet for accessing and managing MoodEntry entities.
        /// </summary>
        public DbSet<MoodEntry> MoodEntries => Set<MoodEntry>();

        /// <summary>
        /// Initializes a new instance of the MoodLiftDbContext class.
        /// </summary>
        /// <param name="options">The options to be used by the context.</param>
        public MoodLiftDbContext(DbContextOptions<MoodLiftDbContext> options) : base(options) { }

        /// <summary>
        /// Configures the database model and entity relationships.
        /// </summary>
        /// <param name="b">The model builder to use for configuration.</param>
        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<MoodEntry>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.GoogleUserId).IsRequired();
                e.Property(x => x.CreatedAtUtc).IsRequired();
                e.Property(x => x.UpdatedAtUtc).IsRequired();

                e.HasIndex(x => new { x.GoogleUserId, x.CreatedAtUtc });

                e.Property(x => x.PrimaryEmotion);
                e.Property(x => x.Symptoms);
                e.Property(x => x.CopingStrategies);
            });
        }

        /// <summary>
        /// Saves all changes made in this context to the database with automatic auditing.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        /// <remarks>
        /// Automatically updates CreatedAtUtc and UpdatedAtUtc timestamps for auditable entities.
        /// </remarks>
        public override int SaveChanges()
        {
            ApplyAudit();
            return base.SaveChanges();
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database with automatic auditing.
        /// </summary>
        /// <param name="ct">Optional cancellation token to cancel the operation.</param>
        /// <returns>The number of state entries written to the database.</returns>
        /// <remarks>
        /// Automatically updates CreatedAtUtc and UpdatedAtUtc timestamps for auditable entities.
        /// </remarks>
        public override Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            ApplyAudit();
            return base.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Applies automatic auditing to entities being added or modified.
        /// </summary>
        /// <remarks>
        /// For new entities:
        /// - Sets both CreatedAtUtc and UpdatedAtUtc to current UTC time
        /// For modified entities:
        /// - Updates UpdatedAtUtc using the entity's Touch method
        /// </remarks>
        private void ApplyAudit()
        {
            var utcNow = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAtUtc = utcNow;
                    entry.Entity.UpdatedAtUtc = utcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    // Polymorphism
                    entry.Entity.Touch(utcNow);
                }
            }
        }
    }
}
