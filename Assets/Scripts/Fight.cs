using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Fight : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Start");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RhythmClient.Instance.Pressed();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            RhythmClient.Instance.Released();   
        }

    }
}

