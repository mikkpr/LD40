using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmTestScript : MonoBehaviour
{
    public Unit unit;

    void Start()
    {
        RhythmEngine.GetTagged().AddMarching(unit);
    }
}
