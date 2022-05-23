using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Put this on the button itself
public class PhysicalButton : MonoBehaviour
{
    public FloatConst buttonDepth;
    public FloatConst pressTime;
    public UnityEvent OnPress;
    [SerializeField, ReadOnly] private Vector3 unpressedPosition;
    [SerializeField, ReadOnly] private Vector3 pressedPosition;
    [SerializeField, ReadOnly] private bool pressed = false;
    private bool transitionToPress = false;
    private bool transitionToUnpress = false;
    private float t = 0;
    public bool Pressed => pressed;

    private void Awake()
    {
        if (!pressed)
        {
            unpressedPosition = this.transform.localPosition;
            pressedPosition = unpressedPosition - (this.transform.up * buttonDepth.Value);
        }
        else
        {
            pressedPosition = this.transform.localPosition;
            unpressedPosition = pressedPosition + (this.transform.up * buttonDepth.Value);
        }
    }

    private void Press()
    {
        if (pressed == true)
        {
            return;
        }

        transitionToPress = true;
        pressed = true;
        t = 0;
        OnPress.Invoke();
    }

    public void ResetButton()
    {
        pressed = false;
        transitionToUnpress = true;
        t = 0;
    }

    private void Update()
    {
        if (transitionToPress)
        {
            this.transform.localPosition = Vector3.Lerp(unpressedPosition, pressedPosition, t);
            t += Time.deltaTime / pressTime.Value;
            if (this.transform.localPosition == pressedPosition)
            {
                transitionToPress = false;
            }
        }
        else if (transitionToUnpress)
        {
            this.transform.localPosition = Vector3.Lerp(pressedPosition, unpressedPosition, t);
            t += Time.deltaTime / pressTime.Value;
            if (this.transform.localPosition == unpressedPosition)
            {
                transitionToUnpress = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (pressed)
        {
            return;
        }
        if (!collision.transform.CompareTag("Player"))
        {
            return;
        }

        if (Player.Instance.modules.groundPoundBehavior.Active)
        {
            Press();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (pressed)
        {
            return;
        }
        if (!collision.transform.CompareTag("Player"))
        {
            return;
        }

        if (Player.Instance.modules.groundPoundBehavior.Active)
        {
            Press();
        }
    }
}