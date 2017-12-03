using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGroup : MonoBehaviour {

    public float locationUpdateDelta = 0.5f;

    private List<Unit> unitList = null;
    private Collider2D col = null;
    private float lastLocationUpdate = float.NegativeInfinity;

    void Awake () {
        unitList = new List<Unit>();
        col = GetComponent<Collider2D>();
    }

    void Update () {
        float t = Time.time;
        if (lastLocationUpdate < t) {
            if (unitList.Count > 0) {
                Vector3 bboxMin = col.bounds.min;
                Vector3 bboxMax = col.bounds.max;

                float bboxHeight = bboxMax.y - bboxMin.y;
                float bboxWidth = bboxMax.x - bboxMin.x;
                float heightDelta = bboxHeight / (unitList.Count + 1);

                // Update targets for units in group
                for (int i = 0; i < unitList.Count; ++i) {
                    Unit u = unitList[i];
                    float widthDelta = bboxWidth / (u.startHealth - 1);
                    float localX = bboxMin.x + (u.health - 1) * widthDelta;
                    float localY = bboxMin.y + (i + 1) * heightDelta;
                    Vector3 localPosition = new Vector3(localX, localY, -col.transform.position.z);
                    u.SetInGroupTarget(localPosition);
                }
            }

            lastLocationUpdate = t + locationUpdateDelta;
        }
    }

    public void AddUnit(Unit u) {
        unitList.Add(u);
    }

    public void RemoveUnit(Unit u) {
        unitList.Remove(u);
    }
}
