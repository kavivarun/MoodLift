using MoodLift.Core.Enum;
using System.ComponentModel.DataAnnotations;


namespace MoodLift.Core.Entities
{
    public class MoodEntry : AuditableEntity
    {
        [Range(0, 10)] public int MoodScore { get; set; }                                   
        public PrimaryEmotion PrimaryEmotion { get; set; }

        public SymptomFlags Symptoms { get; set; } = SymptomFlags.None;

        [Range(0, 24)] public int SleepHours { get; set; }
        [Range(0, 10)] public int EnergyLevel { get; set; }
        [Range(0, 100)] public int CaffeineDrinks { get; set; }

        [Range(0, 10)] public int StressScore { get; set; }

        public CopingStrategyFlags CopingStrategies { get; set; } = CopingStrategyFlags.None;

        // Free text fields
        [MaxLength(4000)] public string? Notes { get; set; }

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
