using SteamTrain;
using UnityEngine;
using TMPro;

public class UILeaderboardEntry : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rank;
    [SerializeField] TextMeshProUGUI user;
    [SerializeField] TextMeshProUGUI time;

    bool hasData = false;

    public void Reset(){
        hasData = false;
    }
    public void SetEntry(LeaderboardEntry data){
        rank.text = data.rank.ToString();
        user.text = data.playerName;
        time.text = LevelTimer.GetTimeString(data.timeSeconds);
        hasData = true;
    }
    public void ScrollCellIndex(int idx)
    {
        LeaderboardEntry entry = UILeaderboard.Instance.GetEntry(idx);
        if (entry != null) {
            SetEntry(UILeaderboard.Instance.GetEntry(idx));
        }else {
            SetEntry(new LeaderboardEntry());
        }
    }
}
