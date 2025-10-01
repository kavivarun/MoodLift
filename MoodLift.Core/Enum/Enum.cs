namespace MoodLift.Core.Enum
{
    [System.Flags]
    public enum SymptomFlags
    {
        None = 0,
        RacingThoughts = 1 << 0,
        LowEnergy = 1 << 1,
        Irritability = 1 << 2,
        AppetiteChanges = 1 << 3,
        PoorConcentration = 1 << 4,
        ChestTightness = 1 << 5,
        Restlessness = 1 << 6,
        Insomnia = 1 << 7
    }

    public enum PrimaryEmotion
    {
        Happy, Calm, Anxious, Sad, Angry, Stressed, Tired, Excited, Overwhelmed, Other
    }

    public enum MovementLevel
    {
        No = 0, Light = 1, Moderate = 2, Intense = 3
    }

    [System.Flags]
    public enum CopingStrategyFlags
    {
        None = 0,
        TalkingToSomeone = 1 << 0,
        BreathingExercises = 1 << 1,
        Journaling = 1 << 2,
        ListeningToMusic = 1 << 3,
        GoingOutside = 1 << 4,
        EatingComfortFood = 1 << 5,
        DistractingMyself = 1 << 6
    }
}
