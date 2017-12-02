using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGroup : MonoBehaviour {

    private List<Unit> unitList = null;
    private Collider2D col = null;
    private float lastLocationUpdate = float.NegativeInfinity;

    void Awake () {
        unitList = new List<Unit>();
        col = GetComponent<Collider2D>();
    }

    void Start() {
        Debug.Log("center" + col.bounds.center);
        Debug.Log("extents" + col.bounds.extents);
        Debug.Log("size" + col.bounds.size);
        Debug.Log("min" + col.bounds.min);
        Debug.Log("max" + col.bounds.max);
    }

    void Update () {
        float t = Time.time;
        if (lastLocationUpdate < t) {
            if (unitList.Count > 0) {
                Vector3 bboxMin = col.bounds.min;
                Vector3 bboxMax = col.bounds.max;

                int n = unitList.Count + 1;
                float bboxHeight = bboxMax.y - bboxMin.y;
                float bboxWidth = bboxMax.x - bboxMin.x;

                float heightDelta = bboxHeight / n;

                // Update targets for units in group
                foreach (Unit u in unitList) {
                    //u.inGroupTarget =
                }
            }

            lastLocationUpdate = t;
        }
    }

    public void AddUnit(Unit u) {
        unitList.Add(u);
    }

    public void RemoveUnit(Unit u) {
        unitList.Remove(u);
    }
}
