using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class FallingBackground : MonoBehaviour
{
    const float minY = 6.0f;
    const float maxY = -0.8f;
    const float duration = 3 * 60;
    const float tick = (minY - maxY) / duration;

    void Start()
    {
        transform.position = new Vector3(transform.position.x, minY);
    }

    void Update()
    {
        if (transform.position.y > maxY)
        {
            var y = minY - (tick * Time.time);
            transform.position = new Vector3(transform.position.x, y);
        }
    }
}
