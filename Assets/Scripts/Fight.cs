using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Timers;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct UnitSpec
{
    public GameObject prefab;
    public float spawnTime;
    public int health;
    public float interval;
    public float offset;
    public bool sequence;
    public Vector3 scrollDirection;
}

public class Fight : MonoBehaviour
{
    public float minY = 0.0f;
    public float maxY = 4.0f;
    public UnitSpec[] unitSpecs;
    public float despawnDelta = 10.0f;

    private List<Unit> units = null;

    void Awake()
    {
        units = new List<Unit>();
        foreach (UnitSpec spec in unitSpecs) {
            // Assume Unit Specification is properly initialized
            units.Add(SpawnUnit(spec));
        }
    }

    void Start()
    {
        SoundManager.instance.PlayMusic();

        foreach (Unit u in units) {
            RhythmEngine.GetTagged().AddMarching(u);
        }
    }

    void Update()
    {
        for (int i = units.Count - 1; i >= 0; --i) {
            Unit u = units[i];
            if (u.transform.localPosition.x < 1.0f) {
                RhythmEngine.GetTagged().RemoveMarching(u);
                u.Kill(despawnDelta);
                units.RemoveAt(i);
            }
        }
        if (units.Count == 0) {
            SceneManager.LoadScene("Lose");
        }
    }

    Unit SpawnUnit(UnitSpec spec)
    {
        GameObject obj = Instantiate(spec.prefab, transform, false);
        obj.transform.position += new Vector3(0.0f, UnityEngine.Random.Range(minY, maxY), 0.0f) + -spec.scrollDirection * spec.spawnTime;
        Unit unit = obj.GetComponent<Unit>();
        unit.scrollDirection = spec.scrollDirection;
        unit.interval = spec.interval;
        unit.offset = spec.offset;
        unit.sequence = spec.sequence;
        unit.health = spec.health;

        return unit;
    }
}
