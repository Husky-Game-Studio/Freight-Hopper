using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3[] waypoints;
    private int index = 0;
    [SerializeField] private bool loop;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 tolerance;
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
                UnityEditor.Handles.DrawDottedLine(waypoints[i], waypoints[i + 1], 50 / speed);
                UnityEditor.Handles.color = Color.white;
            }
            UnityEditor.Handles.DrawWireCube(waypoints[i], tolerance);
        }
        if (loop)
        {
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawDottedLine(waypoints[0], waypoints[waypoints.Length - 1], 50 / speed);
        }
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireCube(waypoints[index], tolerance);
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

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = this.transform;
        }
    }

    private void OnCollisionExit(UnityEngine.Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }

    private void MoveObject()
    {
        Vector3 heading = waypoints[index] - transform.position;
        transform.position += (heading / heading.magnitude) * speed * Time.deltaTime;
        if (Mathf.Abs(heading.x) < tolerance.x && Mathf.Abs(heading.y) < tolerance.y && Mathf.Abs(heading.z) < tolerance.z)
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