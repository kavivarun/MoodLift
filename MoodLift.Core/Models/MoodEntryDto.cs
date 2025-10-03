using MoodLift.Core.Enum;

namespace MoodLift.Core.Models
{
    public record MoodEntryDto(
        int MoodScore,
        PrimaryEmotion PrimaryEmotion,
        SymptomFlags Symptoms,
        int SleepHours,
        int EnergyLevel,
        int CaffeineDrinks,
        int StressScore,
        CopingStrategyFlags CopingStrategies, 
        string? Notes
    );
}
