namespace MoodLift.Core.Entities
{
    /// <summary>
    /// Abstract base class for entities that require audit information.
    /// </summary>
    public abstract class AuditableEntity
    {
        /// <summary>
        /// Unique identifier for the entity.
        /// Automatically generates a new GUID when an entity is instantiated.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The Google User ID of the user who owns this entity.
        /// This is used for authentication and authorization purposes.
        /// </summary>
        public string GoogleUserId { get; set; } = default!;

        /// <summary>
        /// UTC timestamp when the entity was created.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// UTC timestamp when the entity was last updated.
        /// Updated automatically through the Touch method when changes are made.
        /// </summary>
        public DateTime UpdatedAtUtc { get; set; }

        /// <summary>
        /// Updates the entity's last modified timestamp.
        /// This method should be called whenever the entity is modified.
        /// </summary>
        /// <param name="utcNow">The current UTC timestamp to set as the update time</param>
        public virtual void Touch(DateTime utcNow) => UpdatedAtUtc = utcNow;
    }
}
