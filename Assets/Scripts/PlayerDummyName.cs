using UnityEngine;
using TMPro;

public class PlayerDummyName : MonoBehaviour
{
    [SerializeField] TextMeshPro textMeshProUGUI;
    Camera cam;
    private void OnEnable()
    {
        cam = Camera.main;
    }
    public void SetText(string txt){
        textMeshProUGUI.text = txt;
    }

    public void Update()
    {
        textMeshProUGUI.transform.LookAt(cam.transform);
    }
}
