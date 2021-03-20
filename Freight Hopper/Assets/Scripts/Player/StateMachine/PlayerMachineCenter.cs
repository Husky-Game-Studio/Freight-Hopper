/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// State Machine help from these urls:
// https://www.youtube.com/watch?v=5PTd0WdKB-4&list=PL3Z46QHED2JVPi0YHXVcNCqbOKRAO7sxB&index=2
// https://www.youtube.com/watch?v=G1bd75R10m4&list=PL3Z46QHED2JVPi0YHXVcNCqbOKRAO7sxB&index=2
// https://www.youtube.com/watch?v=nnrOhb5UdRc

public abstract class PlayerMachineCenter : MonoBehaviour
{
    private PlayerMode playerMode = PlayerMode.idle;

    public enum PlayerMode
    { 
        idle,
        run,
        jump,
        fall
    }

    private void Update()
    {
        switch (PlayerMode)
        {
            case PlayerMode.idle:
                DoIdle();
                break;

            case PlayerMode.run:
                DoRun();
                break;

            case PlayerMode.jump:
                DoJump();
                break;

            case PlayerMode.fall:
                DoFall();
                break;

            default:
                break;
        }
    }

    private void DoIdle() { }
    private void DoRun() { }
    private void DoJump() { }
    private void DoFall() { }


}
*/