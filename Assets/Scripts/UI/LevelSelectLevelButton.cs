using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;

public class LevelSelectLevelButton : MonoBehaviour, IPointerClickHandler
{
    private Button button;
    public static HashSet<LevelSelectLevelButton> allButtons = new HashSet<LevelSelectLevelButton>();
    private bool unlocked = true;
    private int personalID;
    public static int currentID = 1;
    private RectTransform selector;
    public bool Unlocked => unlocked;
    private bool nextWorldButton;

    private void OnDisable()
    {
        allButtons.Clear();
    }

    public void Initialize(string sceneName, RectTransform selector, int id, bool unlockedStatus, Action buttonAction = null)
    {
        button = GetComponent<Button>();
        button.interactable = false;
        allButtons.Add(this);
        if (button.onClick.GetPersistentEventCount() > 0)
        {
            button.onClick.RemoveAllListeners();
        }
        if(buttonAction == null)
        {
            button.onClick.AddListener(delegate { SceneLoader.LoadLevel(sceneName); });
        }
        else
        {
            button.onClick.AddListener(delegate { buttonAction(); });
            nextWorldButton = true;
        }
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
        selector.SetParent(button.transform);
        selector.transform.localPosition = Vector3.up * -20;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EnableButton();
    }

    public void EnableButton()
    {

        foreach (LevelSelectLevelButton button in allButtons)
        {
            button.DisableButton();
        }
        currentID = personalID;
        if (unlocked)
        {
            button.interactable = true;
            if (nextWorldButton)
            {
                button.onClick.Invoke();
            }
        }
        
        
        if (!nextWorldButton)
        {
            MoveSelector();
        }
        
    }

    public void DisableButton()
    {
        button.interactable = false;
    }
}