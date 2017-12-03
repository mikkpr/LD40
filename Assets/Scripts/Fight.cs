using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Fight : MonoBehaviour
{
    public GameObject pikemanUnit;

    void Start()
    {
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
        pikemanUnit.SetSpawnPositionAt(UnitPosition.Farthest);
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
    public static void SetSpawnPositionAt(this GameObject gm, UnitPosition position)
    {
        Debug.Log("spawn");
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

        var script = gm.GetComponent<Unit>();
        script.scrollDirection = new Vector3(-2, 0, 0);

        gm.transform.position = positionVector;
        gm.transform.localScale = scaleVector;
    }
}