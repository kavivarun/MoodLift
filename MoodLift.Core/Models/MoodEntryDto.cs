using MoodLift.Core.Enum;

namespace MoodLift.Core.Models
{
    /// <summary>
    /// Represents a data transfer object for capturing and transferring mood entry information.
    /// This record encapsulates various aspects of a user's emotional and physical state at a specific point in time,
    /// including mood metrics, physical symptoms, lifestyle factors, and coping mechanisms.
    /// </summary>
    /// <param name="MoodScore">A numerical value representing the user's overall mood, typically on a scale (e.g., 1-10 where higher numbers indicate better mood)</param>
    /// <param name="PrimaryEmotion">The dominant emotion being experienced (e.g., Happy, Sad, Anxious)</param>
    /// <param name="Symptoms">A combination of physical or mental symptoms using flag enums, allowing multiple selections</param>
    /// <param name="SleepHours">Number of hours slept in the previous night, used to track sleep patterns</param>
    /// <param name="EnergyLevel">Current energy level on a scale (e.g., 1-10 where higher numbers indicate more energy)</param>
    /// <param name="CaffeineDrinks">Number of caffeinated beverages consumed, used to track stimulant intake</param>
    /// <param name="StressScore">Perceived stress level on a scale (e.g., 1-10 where higher numbers indicate more stress)</param>
    /// <param name="CopingStrategies">Methods being used to manage mood and stress, supports multiple selections via flags</param>
    /// <param name="Notes">Optional free-form text for additional context or personal observations</param>
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
