using System;

// It just triggers these events with the information. Usually level
public static class EventBoat
{
    public static Action<LevelCompleteData> OnLevelComplete = delegate { };
    public static Action<string>            SeenRoberto     = delegate { }; // levelID
    public static Action<int>               RobertoCount    = delegate { }; // Roberto #
}

public struct LevelCompleteData
{
    public enum InvalidationReason
    {
        None,
        ShortTime,
        CollisionEnabled
    }

    public string Level;
    public int World;
    public float Time;
    public InvalidationReason LevelInvalidationReason;
}