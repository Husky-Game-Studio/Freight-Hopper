using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ExplosiveHazardBehavior : MonoBehaviour
{
    [SerializeField] private GameObject stand;
    private BoxCollider triggerCollider;
    bool detonated = false;

    // Start is called before the first frame update
    void Start()
    {
        triggerCollider = stand.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (detonated)
        {
            Debug.Log("collided");
        }

        //Debug.Log("test");
    }

    private void OnCollisionEnter(Collision other)
    {
        this.detonated = true;
        Debug.Log("Detonated");
    }
}
