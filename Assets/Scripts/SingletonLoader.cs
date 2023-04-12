using UnityEngine;
using UnityEngine.SceneManagement;

public static class SingletonLoader
{
    const string singletonSceneName = "SingletonLoader";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RegisterActiveSceneListener()
    {
        SceneManager.LoadScene(singletonSceneName, LoadSceneMode.Additive);
    }
}
