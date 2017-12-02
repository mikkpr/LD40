using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Fight : MonoBehaviour
{
    Vector3 original;

    void Start()
    {
        Debug.Log("Start");
        original = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space down");
            //SoundManager.instance.PlayMusic("Music_1");
            transform.position = new Vector3(original.x + 1, original.y - 1);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Space up");
            transform.position = original;
        }

    }
}

