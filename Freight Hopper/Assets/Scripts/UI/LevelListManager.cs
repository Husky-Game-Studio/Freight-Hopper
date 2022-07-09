using UnityEngine;

public class LevelListManager : MonoBehaviour
{
    [SerializeField] private Transform levelList;
    [SerializeField] private RectTransform selector;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private WorldListMetaData worlds;
    [SerializeField] private int currentWorldIndex;

    public void SetCurrentWorld(int index)
    {
        currentWorldIndex = index;
        selector.SetParent(this.transform.parent);
        CreateButtons(LevelSelectLevelButton.currentID-1);
    }

    public WorldMetaData CurrentWorld => worlds.Worlds[currentWorldIndex];

    private void OnEnable()
    {
        SetCurrentWorld(0);
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