using System;

namespace SteamTrain
{
    public static class SteamBus
    {
        // For leaderboards
        public static Action OnNewBestTime = delegate { };
        public static Action OnTimeUploaded = delegate { };

        // For multiplayer
        public static Action<SteamP2PManager.P2PPlayerInfo> OnPlayerJoinedGame = delegate { }; // playerName
        public static Action<SteamP2PManager.P2PPlayerInfo> OnPlayerLeftGame = delegate { }; // playerName
    }
}
