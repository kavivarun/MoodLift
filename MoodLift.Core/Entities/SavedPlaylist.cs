using System.ComponentModel.DataAnnotations;

namespace MoodLift.Core.Entities
{
    /// <summary>
    /// Stores a mood-based Spotify playlist generated inside MoodLift.
    /// </summary>
    public class SavedPlaylist : AuditableEntity
    {
        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string SearchQuery { get; set; } = string.Empty;

        [MaxLength(600)]
        public string Reason { get; set; } = string.Empty;

        [MaxLength(120)]
        public string WorkplaceUse { get; set; } = string.Empty;

        [Required]
        public string TrackIdsJson { get; set; } = "[]";
    }
}
