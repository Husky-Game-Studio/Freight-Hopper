using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restarter : MonoBehaviour
{
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K)){
            Ore.ActiveScene.Coroutines.Run(Restart(400));
        }
    }
    #endif

    IEnumerator Restart(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            LevelController instance = LevelController.Instance;
            while (instance == null)
            {
                yield return null;
                instance = LevelController.Instance;
            }
            instance.Respawn();
            Debug.Log("Count " + i);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
