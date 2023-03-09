using System.Collections;
using UnityEngine;
using TMPro;
using SteamTrain;
using Ore;

public class PlayerMessageUpdater : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeLength;
    [SerializeField] float messageLength;

    private void Awake()
    {
        SteamBus.OnPlayerLeftGame += LeaveMessage;
        SteamBus.OnPlayerJoinedGame += JoinMessage;
        EventBoat.OnLevelComplete += InvalidTime;
    }
    private void OnDestroy()
    {
        SteamBus.OnPlayerLeftGame -= LeaveMessage;
        SteamBus.OnPlayerJoinedGame -= JoinMessage;
        EventBoat.OnLevelComplete -= InvalidTime;
    }

    void InvalidTime(LevelCompleteData data){
        if(data.LevelInvalidationReason == LevelCompleteData.InvalidationReason.CollisionEnabled){
            StartCoroutine(ShowMessage("Disable Player Collision To Submit Leaderboard Times", 3, Color.red));
        }
    }

    void JoinMessage(SteamP2PManager.P2PPlayerInfo name) {
        StartCoroutine(ShowMessage($"{name.pname} joined", messageLength, Color.white));
    }
    void LeaveMessage(SteamP2PManager.P2PPlayerInfo name) {
        StartCoroutine(ShowMessage($"{name.pname} left", messageLength, Color.white));
    }

    public IEnumerator ShowMessage(string message, float length, Color color){
        messageText.text = message;
        messageText.color = color;
        yield return FadeIn();
        yield return new WaitForSecondsRealtime(length);
        yield return FadeOut();
    }
    
    IEnumerator FadeIn(){
        canvasGroup.alpha = 0;
        float current = 0;
        float t = 0;
        while (current < fadeLength) {
            t = Mathf.InverseLerp(0, fadeLength, current);
            canvasGroup.alpha = t;
            current += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    IEnumerator FadeOut()
    {
        canvasGroup.alpha = 1;
        float current = fadeLength;
        float t = 0;
        while (current > 0)
        {
            t = Mathf.InverseLerp(0, fadeLength, current);
            canvasGroup.alpha = t;
            current -= Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}