using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SteamTrain
{
    public static class SteamBus
    {
        // For leaderboards
        [Obsolete]public static Action<float> GetBestTime = delegate { };     // The time
        public static Action OnNewBestTime = delegate { };
        // For multiplayer
        public static Action<string> OnPlayerJoinedGame = delegate { }; // playerName
        public static Action<string> OnPlayerLeftGame = delegate { }; // playerName
    }
}
