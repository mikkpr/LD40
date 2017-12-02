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
            Debug.Log("Space down");    
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Space up");
        }

    }
}

