using System;

// It just triggers these events with the information. Usually level
public static class EventBoat
{
    // Sends level name "Format: WorldID LevelID" Assume World ID and Level ID is a string
    public static Action<string> SeenRoberto        = delegate { }; // levelID

    public static Action<string> AddLevelComplete   = delegate { }; // levelID
    public static Action<string, float> NewBestTime = delegate { }; // levelID time
    public static Action<string> BronzeMedalUnlock  = delegate { }; // levelID
    public static Action<string> SilverMedalUnlock  = delegate { }; // levelID
    public static Action<string> GoldMedalUnlock    = delegate { }; // levelID
}
