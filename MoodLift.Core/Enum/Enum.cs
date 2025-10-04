namespace MoodLift.Core.Enum
{
    /// <summary>
    /// Specifies a set of flags that represent common symptoms, allowing multiple symptoms to be combined using bitwise
    /// operations.
    /// </summary>
    /// <remarks>This enumeration is decorated with the <see cref="System.FlagsAttribute"/>, enabling the
    /// combination of multiple symptom values. Use bitwise operators to test for or assign multiple symptoms at once.
    /// This is typically used to represent the presence of one or more symptoms in a single value.</remarks>
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

    /// <summary>
    /// Specifies the set of primary emotional states that can be assigned or detected within the application.
    /// </summary>
    /// <remarks>Use this enumeration to categorize or represent a user's predominant emotion in scenarios
    /// such as mood tracking, feedback, or behavioral analysis. The values cover common emotional states, with <see
    /// cref="PrimaryEmotion.Other"/> available for cases that do not fit the predefined categories.</remarks>
    public enum PrimaryEmotion
    {
        Happy, Calm, Anxious, Sad, Angry, Stressed, Tired, Excited, Overwhelmed, Other
    }

    /// <summary>
    /// Specifies flags that represent various coping strategies which can be combined using bitwise operations.
    /// </summary>
    /// <remarks>This enumeration supports bitwise combination of its values to indicate multiple coping
    /// strategies being used simultaneously. It is decorated with the <see cref="System.FlagsAttribute"/> to enable
    /// flag operations.</remarks>
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
