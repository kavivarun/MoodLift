using Microsoft.EntityFrameworkCore;
using MoodLift.Core.Entities;

namespace MoodLift.Infrastructure.Repositories
{

    public class MoodLiftDbContext : DbContext
    {
        public DbSet<MoodEntry> MoodEntries => Set<MoodEntry>();

        public MoodLiftDbContext(DbContextOptions<MoodLiftDbContext> options) : base(options) { }

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

                // Enums are stored as ints by default with SQLite
                e.Property(x => x.PrimaryEmotion);
                e.Property(x => x.Symptoms);
                e.Property(x => x.CopingStrategies);
            });

        }

        public override int SaveChanges()
        {
            ApplyAudit();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            ApplyAudit();
            return base.SaveChangesAsync(ct);
        }

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
                    // Polymorphism in action: derived entity could override Touch()
                    entry.Entity.Touch(utcNow);
                }
            }
        }
    }

}
