using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Fight : MonoBehaviour
{
    Vector3 original;

    void Start()
    {
        original = transform.position;

        ScrollingScript.MarkPassed += OnMarkPased;
    }

    void OnDestroy()
    {
        
    }

    void Update()
    {
        
    }

    void OnMarkPased(object sender, DistanceEventArgs e)
    {
        print("Marked past (distance: " + e.Distance + ") => do something");
    }

}

