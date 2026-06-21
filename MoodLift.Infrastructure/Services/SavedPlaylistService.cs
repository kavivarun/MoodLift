using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MoodLift.Core.Entities;
using MoodLift.Core.Interfaces;
using MoodLift.Infrastructure.Repositories;

namespace MoodLift.Infrastructure.Services;

public class SavedPlaylistService : ISavedPlaylistService
{
    private readonly MoodLiftDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public SavedPlaylistService(MoodLiftDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Guid> SaveAsync(
        string name,
        string searchQuery,
        string reason,
        string workplaceUse,
        IEnumerable<string> trackIds,
        CancellationToken ct = default)
    {
        var userId = _currentUser.GetGoogleUserId();
        var playlist = new SavedPlaylist
        {
            GoogleUserId = userId,
            Name = Clean(name, 120, "MoodLift playlist"),
            SearchQuery = Clean(searchQuery, 120, "uplifting focus songs"),
            Reason = Clean(reason, 600, "Generated from recent mood check-ins."),
            WorkplaceUse = Clean(workplaceUse, 120, "Personal reset"),
            TrackIdsJson = JsonSerializer.Serialize(trackIds.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().Take(12))
        };

        _db.SavedPlaylists.Add(playlist);
        await _db.SaveChangesAsync(ct);
        return playlist.Id;
    }

    public async Task<List<SavedPlaylist>> GetRecentAsync(int take = 6, CancellationToken ct = default)
    {
        var userId = _currentUser.GetGoogleUserId();
        return await _db.SavedPlaylists
            .AsNoTracking()
            .Where(x => x.GoogleUserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var userId = _currentUser.GetGoogleUserId();
        var playlist = await _db.SavedPlaylists
            .FirstOrDefaultAsync(x => x.Id == id && x.GoogleUserId == userId, ct);

        if (playlist is null)
        {
            return;
        }

        _db.SavedPlaylists.Remove(playlist);
        await _db.SaveChangesAsync(ct);
    }

    private static string Clean(string value, int maxLength, string fallback)
    {
        var cleaned = string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        return cleaned.Length <= maxLength ? cleaned : cleaned[..maxLength];
    }
}
