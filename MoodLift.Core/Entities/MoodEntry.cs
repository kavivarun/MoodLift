using MoodLift.Core.Enum;
using System.ComponentModel.DataAnnotations;


namespace MoodLift.Core.Entities
{
    public class MoodEntry : AuditableEntity
    {
        [Range(0, 10)] public int MoodScore { get; set; }                      
        [MaxLength(64)] public string? MoodWord { get; set; }               
        public PrimaryEmotion PrimaryEmotion { get; set; }

        public SymptomFlags Symptoms { get; set; } = SymptomFlags.None;

        [Range(0, 24)] public int SleepHours { get; set; }
        [Range(0, 10)] public int EnergyLevel { get; set; }
        [Range(0, 100)] public int CaffeineDrinks { get; set; }

        public MovementLevel Movement { get; set; }

        [Range(0, 10)] public int StressScore { get; set; }
        [MaxLength(256)] public string? StressCause { get; set; }

        public CopingStrategyFlags CopingStrategies { get; set; } = CopingStrategyFlags.None;

        // Free text fields
        [MaxLength(2000)] public string? NextActions { get; set; }
        [MaxLength(128)] public string? PositiveThing { get; set; }
        [MaxLength(4000)] public string? Notes { get; set; }

        // One-to-many child collection (generics-based collection List<T>)
        public List<GratitudeItem> Gratitudes { get; set; } = new();

        // Polymorphism via overloaded constructors (useful factory-shorthand)
        public MoodEntry() { }
        public MoodEntry(int moodScore, int energyLevel, int stressScore)
        {
            MoodScore = moodScore;
            EnergyLevel = energyLevel;
            StressScore = stressScore;
        }
    }
}
