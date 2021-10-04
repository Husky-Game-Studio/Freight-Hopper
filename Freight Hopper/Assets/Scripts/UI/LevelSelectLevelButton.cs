using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LevelSelectLevelButton : MonoBehaviour, IPointerClickHandler
{
    private Button button;
    public static HashSet<LevelSelectLevelButton> allButtons = new HashSet<LevelSelectLevelButton>();
    private bool unlocked = true;
    private int personalID;
    public static int currentID;
    private RectTransform selector;

    private void OnEnable()
    {
        button = GetComponent<Button>();
        button.interactable = false;
        allButtons.Add(this);
        MoveSelector();
    }

    private void OnDisable()
    {
        allButtons.Clear();
    }

    public void Initialize(string sceneName, RectTransform selector, int id, bool unlockedStatus)
    {
        if (button.onClick.GetPersistentEventCount() > 0)
        {
            button.onClick.RemoveAllListeners();
        }
        button.onClick.AddListener(delegate { SceneLoader.LoadLevel(sceneName); });
        this.selector = selector;

        string name = id.ToString();
        this.gameObject.name = name;
        GetComponentInChildren<TextMeshProUGUI>().text = name;
        unlocked = unlockedStatus;
        personalID = id;
        if (!unlocked)
        {
            var colors = button.colors;
            colors.disabledColor = Color.black;
            button.colors = colors;
        }
    }

    private void MoveSelector()
    {
        if (button.interactable)
        {
            selector.transform.position = this.transform.position - (Vector3.up * 25);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EnableButton();
    }

    public void EnableButton()
    {
        if (!unlocked)
        {
            return;
        }
        foreach (LevelSelectLevelButton button in allButtons)
        {
            button.DisableButton();
        }
        button.interactable = true;
        currentID = personalID;
        MoveSelector();
    }

    public void DisableButton()
    {
        button.interactable = false;
    }
}