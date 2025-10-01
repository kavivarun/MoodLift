using System.ComponentModel.DataAnnotations;


namespace MoodLift.Core.Entities
{
    public class GratitudeItem
    {
        public int Id { get; set; }
        public Guid MoodEntryId { get; set; }
        public MoodEntry MoodEntry { get; set; } = default!;
        [MaxLength(128)] public string Text { get; set; } = default!;
        public int DisplayOrder { get; set; }
    }
}
