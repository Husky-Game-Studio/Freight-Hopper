using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteamTrain
{
    public class LeaderboardEntry
    {
        public int rank;
        public string playerName;
        public float timeSeconds;
        public ulong steamID;

        public override string ToString()
        {
            return $"{steamID} - [{rank}] [{playerName}] [{timeSeconds}]";
        }

        public LeaderboardEntry(LeaderboardEntry other)
        {
            Copy(other);
        }
        public void Copy(LeaderboardEntry other){
            this.rank = other.rank;
            this.playerName = other.playerName;
            this.timeSeconds = other.timeSeconds;
            this.steamID = other.steamID;
        }
        public LeaderboardEntry()
        {
        }
    }
}
