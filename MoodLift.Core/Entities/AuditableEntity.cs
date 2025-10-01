
namespace MoodLift.Core.Entities
{
    public abstract class AuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Google user ID (OpenID "sub" or NameIdentifier claim)
        public string GoogleUserId { get; set; } = default!;

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        // Virtual "touch" for polymorphic behavior (can be overridden by derived types if needed)
        public virtual void Touch(DateTime utcNow) => UpdatedAtUtc = utcNow;
    }
}
