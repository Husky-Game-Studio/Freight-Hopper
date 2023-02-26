using System;

// It just triggers these events with the information. Usually level
public static class EventBoat
{
    [Obsolete] public static Action<string> AddLevelComplete   = delegate { }; // levelID
    [Obsolete] public static Action<string, float> NewBestTime = delegate { }; // levelID time
    [Obsolete] public static Action<string> BronzeMedalUnlock  = delegate { }; // levelID
    [Obsolete] public static Action<string> SilverMedalUnlock  = delegate { }; // levelID
    [Obsolete] public static Action<string> GoldMedalUnlock    = delegate { }; // levelID

    public static Action<LevelCompleteData> OnLevelComplete = delegate { };
    public static Action<string>            SeenRoberto     = delegate { }; // levelID

    // For leaderboards
    //public static Action<float>     GetBestTime         = delegate { };     // The time
    //public static Action            OnNewBestTime       = delegate { };


    // For multiplayer
    //public static Action<string>    OnPlayerJoinedGame  = delegate { }; // playerName
    //public static Action<string>    OnPlayerLeavedGame  = delegate { }; // playerName
}

public struct LevelCompleteData
{
    public string Level;
    public int World;
    public float Time;
}