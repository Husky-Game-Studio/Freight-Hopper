using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Level/Meta Data"), System.Serializable]
public class LevelMetaData : ScriptableObject
{
    [SerializeField] private string title;
    [SerializeField] private Texture2D image;
    [SerializeField] private int levelID;
    [SerializeField] private Optional<string> tutorialSceneName;

    public string Title => title;
    public Texture2D Image => image;
    public int LevelID => levelID;
    public Optional<string> TutorialSceneName => tutorialSceneName;
}