using TMPro;
using UnityEngine;

public class GameVersionUpdater : MonoBehaviour
{
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = "v" + Application.version;
    }
}
