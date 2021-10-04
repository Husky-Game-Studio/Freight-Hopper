using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelListManager : MonoBehaviour
{
    [SerializeField] private Transform levelList;
    [SerializeField] private RectTransform selector;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private WorldMetaData currentWorld;

    public void SetCurrentWorld(WorldMetaData world)
    {
        currentWorld = world;
    }

    private void OnEnable()
    {
        LevelData[] levels = currentWorld.Levels;
        int count = levelList.childCount;

        for (int i = 0; i < count; i++)
        {
            Destroy(levelList.GetChild(i).gameObject);
        }
        GameObject cache = null;
        for (int i = 0; i < levels.Length; i++)
        {
            GameObject go = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity, levelList);
            string name = "";
            if (levels[i] == null)
            {
                name = "MainMenu";
            }
            else
            {
                name = levels[i].name;
            }
            bool unlocked = true;
            if (name.Equals("MainMenu"))
            {
                unlocked = false;
            }
            if (i + 1 == 1)
            {
                cache = go;
            }
            go.GetComponent<LevelSelectLevelButton>().Initialize(name, selector, i + 1, unlocked);
        }
        if (cache != null)
        {
            cache.GetComponent<LevelSelectLevelButton>().EnableButton();
        }
    }
}