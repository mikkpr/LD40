using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmTestScript : MonoBehaviour
{
    public Unit[] units;

    void Start()
    {
        foreach (Unit unit in units)
        {
            RhythmEngine.GetTagged().AddMarching(unit);
        }
    }
}
