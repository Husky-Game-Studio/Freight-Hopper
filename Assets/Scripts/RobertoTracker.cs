using SteamTrain;
using UnityEngine;

public class RobertoTracker : MonoBehaviour
{
    [SerializeField] SoundManager soundManager;
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        string levelName = LevelController.Instance.CurrentLevelName.CurrentLevel();

        LevelAchievementData level = SaveFile.Current.ReadLevelAchievementData(levelName);
        if (level == null)
        {
            Debug.Log("no save data found, creating new one for Roberto");
            level = new LevelAchievementData
            {
                RobertoFound = false,
                MedalIndex = -1
            };
        } else {
           if(level.RobertoFound){
                return;
           }
        }

        level.RobertoFound = true;
        SaveFile.Current.WriteLevelAchievementData(levelName, level);
        EventBoat.SeenRoberto.Invoke(levelName);
    }

    void EmitRobertoSeenSFX(int robertoCount){
        int robertoSoundNumber = ((robertoCount-1) / 2)+1;
        soundManager.Play($"Roberto{robertoSoundNumber}");
    }

    private void OnEnable()
    {
        EventBoat.RobertoCount += EmitRobertoSeenSFX;
    }
    private void OnDisable()
    {
        EventBoat.RobertoCount -= EmitRobertoSeenSFX;
    }
}
