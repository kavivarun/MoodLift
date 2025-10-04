namespace MoodLift.Components.Models
{
    /// <summary>
    /// Represents a response containing up to three individual tips as string values.
    /// </summary>
    public class TipsResponse
    {
        public string Tip1 { get; set; } = string.Empty;
        public string Tip2 { get; set; } = string.Empty;
        public string Tip3 { get; set; } = string.Empty;
    }
}
