using MoodLift.Core.Entities;
using MoodLift.Core.Models;

namespace MoodLift.Core.Interfaces
{
    /// <summary>
    /// Defines operations for creating and retrieving mood entries within the application.
    /// </summary>
    public interface IMoodEntryService
    {
        Task<Guid> CreateAsync(MoodEntryDto dto, CancellationToken ct = default);

        // Method overloading (polymorphism): both provide “recent entries” with different parameters
        Task<List<MoodEntry>> GetRecentAsync(int take = 20, CancellationToken ct = default);
        Task<List<MoodEntry>> GetRecentAsync(TimeSpan within, int take = 20, CancellationToken ct = default);
    }
}
