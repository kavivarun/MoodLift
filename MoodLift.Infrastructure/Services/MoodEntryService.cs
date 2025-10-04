using MoodLift.Core.Entities;
using MoodLift.Core.Interfaces;
using MoodLift.Core.Models;

namespace MoodLift.Infrastructure.Services;

/// <summary>
/// Service for managing mood entries in the application.
/// Handles creating and retrieving mood entries for the current user.
/// </summary>
public class MoodEntryService : IMoodEntryService
{
    private readonly IRepository<MoodEntry> _repo;
    private readonly ICurrentUserService _currentUser;

    /// <summary>
    /// Initializes a new instance of the MoodEntryService class.
    /// </summary>
    /// <param name="repo">Repository for performing CRUD operations on mood entries.</param>
    /// <param name="currentUser">Service to get the current user's Google ID.</param>
    public MoodEntryService(IRepository<MoodEntry> repo, ICurrentUserService currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Creates a new mood entry for the current user.
    /// </summary>
    /// <param name="dto">The mood entry data transfer object containing the entry details.</param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>The ID of the newly created mood entry.</returns>
    /// <remarks>
    /// The method automatically associates the entry with the current user
    /// </remarks>
    public async Task<Guid> CreateAsync(MoodEntryDto dto, CancellationToken ct = default)
    {
        var userId = _currentUser.GetGoogleUserId();

        // Use overloaded constructor (polymorphism) for key numbers
        var entry = new MoodEntry(dto.MoodScore, dto.EnergyLevel, dto.StressScore)
        {
            GoogleUserId = userId,
            PrimaryEmotion = dto.PrimaryEmotion,
            Symptoms = dto.Symptoms,
            SleepHours = dto.SleepHours,
            CaffeineDrinks = dto.CaffeineDrinks,
            CopingStrategies = dto.CopingStrategies,
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes!.Trim()
        };

        await _repo.AddAsync(entry, ct);
        await _repo.SaveChangesAsync(ct);

        return entry.Id;
    }

    /// <summary>
    /// Gets recent mood entries for the current user from the last 30 days.
    /// </summary>
    /// <param name="take">Maximum number of entries to return. Defaults to 20.</param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>A list of recent mood entries, ordered by creation date descending.</returns>
    public Task<List<MoodEntry>> GetRecentAsync(int take = 20, CancellationToken ct = default)
        => GetRecentAsync(TimeSpan.FromDays(30), take, ct); // method overloading

    /// <summary>
    /// Gets recent mood entries for the current user within a specified time window.
    /// </summary>
    /// <param name="within">Time span to look back from current time.</param>
    /// <param name="take">Maximum number of entries to return. Defaults to 20.</param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>A list of mood entries within the specified time window, ordered by creation date descending.</returns>
    public async Task<List<MoodEntry>> GetRecentAsync(TimeSpan within, int take = 20, CancellationToken ct = default)
    {
        var userId = _currentUser.GetGoogleUserId();
        var since = DateTime.UtcNow - within;

        // LINQ lambda with expression tree (filters in DB)
        var items = await _repo.ListAsync(x => x.GoogleUserId == userId && x.CreatedAtUtc >= since, ct);

        // In-memory ordering/take (another lambda)
        return items.OrderByDescending(x => x.CreatedAtUtc).Take(take).ToList();
    }
}
