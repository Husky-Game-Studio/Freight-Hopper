using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentAbilityManager : MonoBehaviour
{
    [SerializeField] private GameObject backgroundAbilityPrefab;
    [SerializeField] private SpriteList abilityIcons;
    [SerializeField] private Color disabledColor;
    [SerializeField] private Color enabledColor;    [SerializeField] private SelectedLevelDataManager selectedLevelDataManager;    [SerializeField] private Button unlockButton;    [SerializeField] private Button lockButton;    [SerializeField, ReadOnly] private LevelData currentLevel;    [SerializeField, ReadOnly] private List<UnityEngine.UI.Toggle> toggles = new List<UnityEngine.UI.Toggle>();
    [SerializeField, ReadOnly]
    private PlayerAbilities.Name[] nameToIndex =
    {
        PlayerAbilities.Name.DoubleJumpBehavior,
        PlayerAbilities.Name.UpwardDashBehavior,
        PlayerAbilities.Name.FullStopBehavior,
        PlayerAbilities.Name.GrapplePoleBehavior,
        PlayerAbilities.Name.BurstBehavior,
    };    private void OnEnable()
    {
        currentLevel = selectedLevelDataManager.CurrentLevelData;
        lockButton.gameObject.SetActive(false);
        unlockButton.gameObject.SetActive(true);
        currentLevel.RestartActiveAbilities();
        CreateToggles();
        SetTogglesActive(false);

        if (unlockButton.onClick.GetPersistentEventCount() > 0)
        {
            unlockButton.onClick.RemoveAllListeners();
        }

        unlockButton.onClick.AddListener(delegate
        {
            SetTogglesActive(true);
            lockButton.gameObject.SetActive(true);
            unlockButton.gameObject.SetActive(false);
        });

        if (lockButton.onClick.GetPersistentEventCount() > 0)
        {
            lockButton.onClick.RemoveAllListeners();
        }
        lockButton.onClick.AddListener(delegate
        {
            RestartToggles();
            SetTogglesActive(false);
            lockButton.gameObject.SetActive(false);
            unlockButton.gameObject.SetActive(true);
        });
    }
    private void SetTogglesActive(bool activeState)
    {
        //Debug.Log("working?");
        for (int i = 0; i < toggles.Count; i++)
        {
            toggles[i].gameObject.SetActive(activeState);
        }
    }    private void CreateToggles()
    {
        int c = toggles.Count;
        if (c > 0)
        {
            for (int i = 0; i < c; i++)
            {
                Destroy(toggles[i].gameObject);
            }
        }
        toggles.Clear();
        Sprite[] sprites = abilityIcons.Sprites;
        for (int i = 0; i < sprites.Length; i++)
        {
            GameObject go = Instantiate(backgroundAbilityPrefab, this.transform);
            UnityEngine.UI.Toggle toggle = go.GetComponent<UnityEngine.UI.Toggle>();
            toggles.Add(toggle);
            Image image = go.transform.GetChild(0).GetComponent<Image>();
            image.sprite = sprites[i];
            UpdateToggle(i);
            if (toggle.onValueChanged.GetPersistentEventCount() > 0)
            {
                toggle.onValueChanged.RemoveAllListeners();
            }
            int number = i;
            toggle.onValueChanged.AddListener(delegate { ToggleToggle(number); });
        }
    }

    private void ToggleToggle(int i)
    {
        //Debug.Log("toggle toggle for " + i + " and length is " + toggles.Count);
        UnityEngine.UI.Toggle toggle = toggles[i];
        Image background = toggle.GetComponent<Image>();
        PlayerAbilities.Name ability = nameToIndex[i];

        if (!toggle.isOn)
        {
            currentLevel.RemoveActiveAbility(ability);
            background.color = disabledColor;
        }
        else
        {
            currentLevel.AddActiveAbility(ability);
            background.color = enabledColor;
        }
    }

    private void RestartToggles()
    {
        currentLevel.RestartActiveAbilities();
        for (int i = 0; i < toggles.Count; i++)
        {
            UpdateToggle(i);
        }
    }

    private void UpdateToggle(int i)
    {
        UnityEngine.UI.Toggle toggle = toggles[i];
        Image background = toggle.GetComponent<Image>();
        PlayerAbilities.Name ability = nameToIndex[i];

        if (currentLevel.ContainsAbility(ability))
        {
            toggle.isOn = true;
            background.color = enabledColor;
        }
        else
        {
            toggle.isOn = false;
            background.color = disabledColor;
        }
    }}