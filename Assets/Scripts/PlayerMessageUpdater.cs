using System.Collections;
using UnityEngine;
using TMPro;
using SteamTrain;

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
    }
    private void OnDestroy()
    {
        SteamBus.OnPlayerLeftGame -= LeaveMessage;
        SteamBus.OnPlayerJoinedGame -= JoinMessage;
    }

    void JoinMessage(SteamP2PManager.P2PPlayerInfo name) {
        StartCoroutine(ShowMessage($"{name.pname} joined"));
    }
    void LeaveMessage(SteamP2PManager.P2PPlayerInfo name) {
        StartCoroutine(ShowMessage($"{name.pname} left"));
    }

    public IEnumerator ShowMessage(string message){
        messageText.text = message;
        yield return FadeIn();
        yield return new WaitForSeconds(messageLength);
        yield return FadeOut();
    }
    
    IEnumerator FadeIn(){
        canvasGroup.alpha = 0;
        float current = 0;
        float t = 0;
        while (current < fadeLength) {
            t = Mathf.InverseLerp(0, fadeLength, current);
            canvasGroup.alpha = t;
            current += Time.deltaTime;
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
            t = Mathf.InverseLerp(fadeLength, 0, current);
            canvasGroup.alpha = t;
            current -= Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}