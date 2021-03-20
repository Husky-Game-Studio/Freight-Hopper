using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// State Machine help from these urls:
// https://www.youtube.com/watch?v=5PTd0WdKB-4&list=PL3Z46QHED2JVPi0YHXVcNCqbOKRAO7sxB&index=2
// https://www.youtube.com/watch?v=G1bd75R10m4&list=PL3Z46QHED2JVPi0YHXVcNCqbOKRAO7sxB&index=2

public abstract class BasicState : MonoBehaviour
{
    protected PlayerSystem playerSystem;

    public BasicState(PlayerSystem playerSystem) {
        this.playerSystem = playerSystem;
    }

    public virtual IEnumerator Start()
    {
        yield break;
    }

    public virtual IEnumerator Attack() { 
        yield break;
    }
    public virtual IEnumerator Heal()
    {
        yield break;
    }
    public virtual IEnumerator Pause()
    {
        yield break;
    }
    public virtual IEnumerator Resume()
    {
        yield break;
    }
}
