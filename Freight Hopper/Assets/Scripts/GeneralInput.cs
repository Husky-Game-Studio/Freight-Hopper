using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralInput : MonoBehaviour
{
    private UserInput input;

    private void Awake()
    {
        input = GetComponent<UserInput>();
    }

    private void Update()
    {
        if (input.Restart())
        { // Optimize this later please
            this.transform.position = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelSettings>().SpawnPosition;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}