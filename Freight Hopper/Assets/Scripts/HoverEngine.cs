using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverEngine : MonoBehaviour
{
    public PID controller;

    public void Initailize(float p, float i, float d)
    {
        controller.Initalize(p, i, d);
    }
}