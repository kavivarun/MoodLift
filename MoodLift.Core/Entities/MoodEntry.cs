using MoodLift.Core.Enum;
using System.ComponentModel.DataAnnotations;


namespace MoodLift.Core.Entities
{
    /// <summary>
    /// Represents a single mood tracking entry, including mood score, primary emotion, symptoms, sleep, energy, stress,
    /// coping strategies, and optional notes.
    /// </summary>
    public class MoodEntry : AuditableEntity
    {
        /// <summary>
        /// The user's self-reported mood score on a scale of 0 to 10.
        /// </summary>
        [Range(0, 10)]
        public int MoodScore { get; set; }

        /// <summary>
        /// The primary emotion the user is experiencing at the time of entry.
        /// </summary>
        [Required]
        public PrimaryEmotion PrimaryEmotion { get; set; }

        /// <summary>
        /// Any physical or mental symptoms the user is experiencing, stored as flags.
        /// </summary>
        public SymptomFlags Symptoms { get; set; } = SymptomFlags.None;

        /// <summary>
        /// Number of hours the user slept in the previous night (0-24).
        /// </summary>
        [Range(0, 24)]
        public int SleepHours { get; set; }

        /// <summary>
        /// User's current energy level on a scale of 0 to 10.
        /// </summary>
        [Range(0, 10)]
        public int EnergyLevel { get; set; }

        /// <summary>
        /// Number of caffeinated drinks consumed.
        /// </summary>
        [Range(0, 100)]
        public int CaffeineDrinks { get; set; }

        /// <summary>
        /// User's current stress level on a scale of 0 to 10.
        /// </summary>
        [Range(0, 10)]
        public int StressScore { get; set; }

        /// <summary>
        /// The coping strategies being employed by the user, stored as flags.
        /// </summary>
        public CopingStrategyFlags CopingStrategies { get; set; } = CopingStrategyFlags.None;

        /// <summary>
        /// Optional additional notes or observations about the mood entry.
        /// </summary>
        [MaxLength(4000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Initializes a new instance of the MoodEntry class with default values.
        /// The base AuditableEntity will initialize with a new GUID and timestamp.
        /// </summary>
        public MoodEntry() { }

        /// <summary>
        /// Initializes a new instance of the MoodEntry class with core mood metrics.
        /// </summary>
        /// <param name="moodScore">Overall mood score (0-10)</param>
        /// <param name="energyLevel">Current energy level (0-10)</param>
        /// <param name="stressScore">Current stress level (0-10)</param>
        public MoodEntry(int moodScore, int energyLevel, int stressScore)
        {
            MoodScore = moodScore;
            EnergyLevel = energyLevel;
            StressScore = stressScore;
        }

        /// <summary>
        /// Updates the entity's timestamp when changes are made.
        /// </summary>
        /// <param name="utcNow">Current UTC timestamp</param>
        public override void Touch(DateTime utcNow) => base.Touch(utcNow);
    }
}
