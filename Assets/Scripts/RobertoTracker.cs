using SteamTrain;
using UnityEngine;

public class RobertoTracker : MonoBehaviour
{
    [SerializeField] SoundManager soundManager;
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        string levelName = LevelController.Instance.CurrentLevelName.CurrentLevel();

        LevelSaveData level = LevelTimeSaveLoader.Load(levelName);
        if (level == null)
        {
            Debug.Log("no save data found, creating new one for Roberto");
            level = new LevelSaveData
            {
                LevelName = levelName,
                MedalIndex = -1,
                RobertoFound = true
            };
        } else {
           if(level.RobertoFound){
                return;
           }
        }

        level.FoundRoberto();
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
