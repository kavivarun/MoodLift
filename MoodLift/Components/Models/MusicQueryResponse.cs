namespace MoodLift.Components.Models
{
    /// <summary>
    /// Gets or sets the original query string used to request music data.
    /// </summary>
    public class MusicQueryResponse
    {
        public string Query { get; set; } = string.Empty;

        public string PlaylistName { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;

        public string WorkplaceUse { get; set; } = string.Empty;
    }
}
