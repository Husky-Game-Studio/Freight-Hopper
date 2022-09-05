using UnityEngine;

public class InGameStates : MonoBehaviour
{
    public static InGameStates Instance;

    States currentState;
    void Awake()
    {
        Instance = this;
        currentState = States.Playing;
        Time.timeScale = 1f;
    }

    public enum States
    {
        Playing,
        Freecam,
        Paused,
        LevelComplete
    }
    public bool StateIs(States targetState)
    {
        return currentState == targetState;
    }
    public void SwitchState(States newState)
    {
        currentState = newState;
    }
}
