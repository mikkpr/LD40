using UnityEngine;
using System.Collections;

public class RhythmClient
{
    public static readonly RhythmClient Instance = new RhythmClient();

    public void Pressed()
    {
        Debug.Log("RhythmClient Pressed");
    }

    public void Released()
    {
        Debug.Log("RhythmClient Released");
    }
}
