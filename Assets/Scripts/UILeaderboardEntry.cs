using SteamTrain;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class UILeaderboardEntry : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rank;
    [SerializeField] TextMeshProUGUI user;
    [SerializeField] TextMeshProUGUI time;

    public void SetEntry(LeaderboardEntry data){
        rank.text = data.rank.ToString();
        user.text = data.playerName;
        time.text = LevelTimer.GetTimeString(data.timeSeconds);
    }
}
