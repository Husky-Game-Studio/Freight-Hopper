using SteamTrain;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UILeaderboardEntry : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rank;
    [SerializeField] TextMeshProUGUI user;
    [SerializeField] TextMeshProUGUI time;
    [SerializeField] Image backgroundImage;

    [SerializeField] Color defaultBackgroundColor;
    [SerializeField] Color defaultTextColor;

    [SerializeField] Color highlightedBackgroundColor;
    [SerializeField] Color highlightedTextColor;

    public void SetEntry(LeaderboardEntry data) {
        rank.text = data.rank.ToString();
        user.text = data.playerName;
        time.text = LevelTimer.GetTimeString(data.timeSeconds);

        if (data.steamID == SteamManager.GetMySteamID()){
            rank.color = highlightedTextColor;
            user.color = highlightedTextColor;
            time.color = highlightedTextColor;
            backgroundImage.color = highlightedBackgroundColor;
        } else {
            rank.color = defaultTextColor;
            user.color = defaultTextColor;
            time.color = defaultTextColor;
            backgroundImage.color = defaultBackgroundColor;
        }
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
