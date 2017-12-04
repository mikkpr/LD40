using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmTestScript : MonoBehaviour
{
    public Unit[] units;
    public Boss boss;

    void Start()
    {
        RhythmEngine rm = RhythmEngine.GetTagged();
        foreach (Unit unit in units)
        {
            rm.AddMarching(unit);
        }
        if (boss != null)
        {
            rm.SetBoss(boss);
        }
    }
}
