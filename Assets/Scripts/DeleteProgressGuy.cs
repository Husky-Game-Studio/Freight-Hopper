using UnityEngine;

public class DeleteProgressGuy : MonoBehaviour
{
    [SerializeField] WorldListMetaData worldListMetaData;
    public void ObliterateProgress()
    {
        if (SaveFile.Instance.DeleteCacheFile())
        {
            SteamTrain.SteamAchievementHandler.ObliterateEverything();
            foreach (var level in worldListMetaData.Worlds[0].Levels)
            {
                LevelCompleteData levelCompleteData = new LevelCompleteData()
                {
                    World = 1,
                    Level = new LevelName(level.name).VersionedCurrentLevel(level),
                };
                LevelComplete.ForceUploadMaxTimeGamer(levelCompleteData);
            }
        }
    }
}
