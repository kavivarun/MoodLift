using MoodLift.Core.Entities;
using MoodLift.Core.Interfaces;
using MoodLift.Core.Models;

namespace MoodLift.Infrastructure.Services;

public class MoodEntryService : IMoodEntryService
{
    private readonly IRepository<MoodEntry> _repo;
    private readonly ICurrentUserService _currentUser;

    public MoodEntryService(IRepository<MoodEntry> repo, ICurrentUserService currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

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

    public Task<List<MoodEntry>> GetRecentAsync(int take = 20, CancellationToken ct = default)
        => GetRecentAsync(TimeSpan.FromDays(30), take, ct); // method overloading

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
