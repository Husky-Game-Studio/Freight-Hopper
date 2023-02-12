using System;

public static class EventBoat
{
    // Sends level name "Format: WorldID LevelID" Assume World ID and Level ID is a string
    public static Action<string> AddLevelComplete   = delegate { }; 
    public static Action<string> SeenRoberto        = delegate { };
    public static Action<string> BronzeMedalUnlock  = delegate { };
    public static Action<string> SilverMedalUnlock  = delegate { };
    public static Action<string> GoldMedalUnlock    = delegate { };
}
