using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralInput : MonoBehaviour
{
    private void Awake()
    {
        UserInput.Input.UserInputMaster.Player.Restart.performed += LevelController.Instance.Respawn;
    }
}