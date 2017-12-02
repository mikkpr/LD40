using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGroup : MonoBehaviour {

    public float groupBeginScreenRatio = 0.1f;
    public float groupEndScreenRatio = 0.5f;

    private List<Unit> groupList = null;

    void Awake () {
        groupList = new List<Unit>();
    }

    void Update () {

    }

    public void AddUnit(Unit u) {
        groupList.Add(u);
    }

    public void RemoveUnit(Unit u) {
        groupList.Remove(u);
    }
}
