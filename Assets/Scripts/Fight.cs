using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Timers;
using System.Runtime.InteropServices;

public class Fight : MonoBehaviour
{
    public GameObject pikemanUnit;

    void Start()
    {
        SoundManager.instance.PlayMusic();
        SpawnUnit(UnitPosition.Initial);
    }

    float nextTime = 32;

    void Update()
    {
        var time = SoundManager.instance.GetCurrentTime();
        //Debug.Log("Current time: " + time);

        if (time > nextTime)
        {
            SpawnUnit();
            nextTime += 32;
        }
    }

    void SpawnUnit(UnitPosition position = UnitPosition.Farthest)
    {
        GameObject newObject = pikemanUnit;
        Unit unit = newObject.SetSpawnPositionAt(position);

        //GCHandle objHandle = GCHandle.Alloc(unit,GCHandleType.WeakTrackResurrection);
        //int address = GCHandle.ToIntPtr(objHandle).ToInt32(); 
        //Debug.Log("Address: " + address);
        try
        {
            RhythmEngine.GetTagged().AddMarching(unit);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }

        Instantiate(newObject);
    }

}


public enum UnitPosition
{
    // Initial is your first unit, no scroll direction 
    // and positioned in the left-center part of your screen
    Initial,
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

        if (position == UnitPosition.Initial)
        {
            positionVector.x = -7.71f;
            positionVector.y = -3.38f;

            scaleVector.x = 0.3f;
            scaleVector.y = 0.3f;
        }
        else if (position == UnitPosition.Farthest)
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

        if (position == UnitPosition.Initial)
        {
            script.scrollDirection = new Vector3(0, 0, 0);    
        }
        else
        {
            script.scrollDirection = new Vector3(-2, 0, 0);    
        }

        return script;
    }
}