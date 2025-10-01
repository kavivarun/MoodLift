using MoodLift.Core.Enum;

namespace MoodLift.Core.Models
{
    public record MoodEntryDto(
        int MoodScore,
        string? MoodWord,
        PrimaryEmotion PrimaryEmotion,
        SymptomFlags Symptoms,
        int SleepHours,
        int EnergyLevel,
        int CaffeineDrinks,
        MovementLevel Movement,
        int StressScore,
        string? StressCause,
        CopingStrategyFlags CopingStrategies,
        string? NextActions,
        List<string> Gratitudes,   
        string? PositiveThing,
        string? Notes
    );
}
