using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Timers;

public class Fight : MonoBehaviour
{
    public GameObject pikemanUnit;

    void Start()
    {
        SoundManager.instance.PlayMusic();
    }

    void OnDestroy()
    {

    }

    float nextTime = 32;

    void Update()
    {
        var time = SoundManager.instance.GetCurrentTime();
        Debug.Log("Current time: " + time);

        if (time > nextTime)
        {
            SpawnUnit();
            nextTime += 32;
        }
    }

    void SpawnUnit()
    {
        Unit unit = pikemanUnit.SetSpawnPositionAt(UnitPosition.Farthest);
        RhythmEngine.GetTagged().AddMarching(unit);

        Instantiate(pikemanUnit);
    }

}


public enum UnitPosition
{
    None,
    Near,
    NearCenter,
    Center,
    Far,
    Farthest
}

public static class GameObjectExtensions
{
    public static Unit SetSpawnPositionAt(this GameObject gm, UnitPosition position)
    {
        var positionVector = new Vector3();
        positionVector.z = (float)-5;

        var scaleVector = new Vector3();
        scaleVector.z = 0;

        if (position == UnitPosition.Farthest)
        {
            scaleVector.x = 0.3f;
            scaleVector.y = 0.3f;

            // 12 is approx. exactly offscreen, will immedicately enter
            positionVector.x = 12;
            positionVector.y = -2;
        }

        gm.transform.position = positionVector;
        gm.transform.localScale = scaleVector;

        var script = gm.GetComponent<Unit>();
        script.scrollDirection = new Vector3(-2, 0, 0);

        return script;
    }
}