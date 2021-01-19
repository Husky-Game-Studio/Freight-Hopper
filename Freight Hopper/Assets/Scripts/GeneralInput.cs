using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralInput : MonoBehaviour
{
    private void Update()
    {
        if (UserInput.Input.Restart())
        { // Optimize this later please
            this.GetComponent<Collision>().Respawn();
        }
    }
}