using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3[] waypoints;
    private int index = 0;
    [SerializeField] private bool loop;
    [SerializeField] private float speed;
    [SerializeField] private float tolerance;
    [SerializeField] private float delayDuration;
    private float currentWaitTime;
    private int currentDirection = 1;

    private void OnDrawGizmos()
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (i != waypoints.Length - 1)
            {
                UnityEditor.Handles.color = Color.yellow;
                UnityEditor.Handles.DrawDottedLine(waypoints[i], waypoints[i + 1], 1f);
                UnityEditor.Handles.color = Color.white;
            }
            UnityEditor.Handles.DrawWireCube(waypoints[i], Vector3.one / 5);
        }
    }

    private void FixedUpdate()
    {
        if (index < waypoints.Length && index >= 0 && transform.position != waypoints[index])
        {
            MoveObject();
        }
        else
        {
            ChangeTarget();
        }
    }

    private void MoveObject()
    {
        Vector3 heading = waypoints[index] - transform.position;
        transform.position += (heading / heading.magnitude) * speed * Time.deltaTime;
        if (heading.magnitude < tolerance)
        {
            transform.position = waypoints[index];
            currentWaitTime = Time.time;
        }
    }

    private void ChangeTarget()
    {
        if (Time.time - currentWaitTime > delayDuration)
        {
            NextTarget();
        }
    }

    private void NextTarget()
    {
        index += currentDirection;

        if (index >= waypoints.Length)
        {
            if (loop)
            {
                index = 0;
                currentDirection = 1;
            }
            else
            {
                currentDirection = -1;
            }
        }
        else if (index <= 0)
        {
            if (!loop)
            {
                currentDirection = 1;
            }
        }
    }
}