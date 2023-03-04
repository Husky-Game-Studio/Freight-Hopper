using System;

namespace SteamTrain
{
    public static class SteamBus
    {
        // For leaderboards
        public static Action OnNewBestTime = delegate { };
        public static Action OnTimeUploaded = delegate { };

        // For multiplayer
        public static Action<string> OnPlayerJoinedGame = delegate { }; // playerName
        public static Action<string> OnPlayerLeftGame = delegate { }; // playerName
    }
}
