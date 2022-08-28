using System.Text;
using UnityEngine;
using TMPro;

// Source: https://www.youtube.com/watch?v=x-C95TuQtf0
public class LevelTimer : MonoBehaviour
{
    private TextMeshProUGUI textTimer;
    private float startTime;

    private void Start()
    {
        startTime = Time.realtimeSinceStartup;
        textTimer = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        textTimer.text = GetTimeString(GetTime());
    }

    public float GetTime()
    {
        return Time.realtimeSinceStartup - startTime;
    }

    public static string GetTimeString(float time)
    {
        int minutes = (int)time / 60;
        float seconds = time % 60;

        StringBuilder sb = new StringBuilder();
        if (minutes > 0)
        {
            if (minutes < 10)
            {
                sb.Append("0");
            }
            sb.Append(minutes);
            sb.Append(":");
        }
        if (seconds < 10)
        {
            sb.Append("0");
        }
        sb.Append(seconds.ToString("f3"));
        return sb.ToString();
    }
}