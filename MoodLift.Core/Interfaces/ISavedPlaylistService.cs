using MoodLift.Core.Entities;

namespace MoodLift.Core.Interfaces
{
    public interface ISavedPlaylistService
    {
        Task<Guid> SaveAsync(string name, string searchQuery, string reason, string workplaceUse, IEnumerable<string> trackIds, CancellationToken ct = default);

        Task<List<SavedPlaylist>> GetRecentAsync(int take = 6, CancellationToken ct = default);

        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
