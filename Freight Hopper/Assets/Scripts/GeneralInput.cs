using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralInput : MonoBehaviour
{
    private void Update()
    {
        if (UserInput.Input.Restart())
        { // Optimize this later please
            this.transform.position = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelSettings>().SpawnPosition;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}