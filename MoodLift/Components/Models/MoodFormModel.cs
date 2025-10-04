using MoodLift.Core.Enum;
using System.ComponentModel.DataAnnotations;

namespace MoodLift.Components.Models
{
    /// <summary>
    /// Represents the data model for a mood tracking form, including mood score, emotions, symptoms, sleep, energy,
    /// stress, and related notes.
    public class MoodFormModel
    {
        [Range(0, 10, ErrorMessage = "Mood must be between 0 and 10.")]
        public int MoodScore { get; set; }

        public PrimaryEmotion PrimaryEmotion { get; set; } = PrimaryEmotion.Calm;
        public SymptomFlags Symptoms { get; set; } = SymptomFlags.None;

        [Range(0, 24, ErrorMessage = "Sleep must be 0–24 hours.")]
        public int SleepHours { get; set; }

        [Range(0, 10, ErrorMessage = "Energy must be 0–10.")]
        public int EnergyLevel { get; set; }

        [Range(0, 10, ErrorMessage = "Caffeine drinks must be 0–10.")]
        public int CaffeineDrinks { get; set; }

        [Range(0, 10, ErrorMessage = "Stress must be 0–10.")]
        public int StressScore { get; set; }

        public string? StressCause { get; set; }
        public CopingStrategyFlags CopingStrategies { get; set; } = CopingStrategyFlags.None;
        public string? Notes { get; set; }
    }
}
