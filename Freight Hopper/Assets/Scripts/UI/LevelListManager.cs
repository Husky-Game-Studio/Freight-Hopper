using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelListManager : MonoBehaviour
{
    [SerializeField] private Transform levelList;
    [SerializeField] private RectTransform selector;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private WorldListMetaData worlds;
    [SerializeField] private int currentWorldIndex;
    [SerializeField] private RectTransform nextWorld;
    [SerializeField] private RectTransform prevWorld;

    public void SetCurrentWorld(int index)
    {
        currentWorldIndex = index;
        selector.SetParent(this.transform.parent);
        CreateButtons(LevelSelectLevelButton.currentID-1);
        CheckNextWorld();
        CheckPrevWorld();
        
        
    }

    public WorldMetaData CurrentWorld => worlds.Worlds[currentWorldIndex];

    private void OnEnable()
    {
        SetCurrentWorld(0);
    }

    private void CheckNextWorld()
    {
        int count = nextWorld.childCount;
        for (int i = 0; i < count; i++)
        {
            Destroy(nextWorld.GetChild(i).gameObject);
        }
        if (currentWorldIndex + 1 >= worlds.Worlds.Count || worlds.Worlds[currentWorldIndex + 1] == null)
        { 
            return; 
        }


        for (int i = 0; i < 3; i++)
        {
            GameObject go = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity, nextWorld);

            go.GetComponent<LevelSelectLevelButton>().Initialize("next", selector, i+1, true, IncrementWorld);
        }
    }

    private void CheckPrevWorld()
    {
        int count = prevWorld.childCount;
        for (int i = 0; i < count; i++)
        {
            Destroy(prevWorld.GetChild(i).gameObject);
        }
        if (currentWorldIndex - 1 < 0 || worlds.Worlds[currentWorldIndex - 1] == null)
        {
            return;
        }


        for (int i = this.CurrentWorld.Levels.Count-1; i >= this.CurrentWorld.Levels.Count-3; i--)
        {
            GameObject go = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity, prevWorld);

            go.GetComponent<LevelSelectLevelButton>().Initialize("prev", selector, i + 1, true, DecrementWorld);
        }
    }

    public void IncrementWorld()
    {
        SetCurrentWorld(currentWorldIndex + 1);
    }
    public void DecrementWorld()
    {
        SetCurrentWorld(currentWorldIndex - 1);
    }

    private void CreateButtons(int startIndex)
    {
        int count = levelList.childCount;
        
        for (int i = 0; i < count; i++)
        {
            Destroy(levelList.GetChild(i).gameObject);
        }
        GameObject cache = null;
        for (int i = 0; i < this.CurrentWorld.Levels.Count; i++)
        {
            GameObject go = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity, levelList);
            string name;
            if (this.CurrentWorld.Levels[i] == null)
            {
                name = "MainMenu";
            }
            else
            {
                name = this.CurrentWorld.Levels[i].name;
            }
            bool unlocked = true;
            if (name.Equals("MainMenu"))
            {
                unlocked = false;
            }

            if (i == startIndex)
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